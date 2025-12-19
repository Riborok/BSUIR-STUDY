library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.NUMERIC_STD.ALL;

entity simon_says is
    Port (
        clk         : in  STD_LOGIC;                      -- 100MHz clock (E3)
        rst         : in  STD_LOGIC;                      -- CPU Reset button (C12) -> Active LOW
        btn_start   : in  STD_LOGIC;                      -- BTNC (E16)
        switches    : in  STD_LOGIC_VECTOR(15 downto 0);  -- Slide switches
        leds        : out STD_LOGIC_VECTOR(15 downto 0);  -- User LEDs
        seg         : out STD_LOGIC_VECTOR(6 downto 0);   -- 7-segment cathodes
        dp          : out STD_LOGIC;                      -- Decimal point
        an          : out STD_LOGIC_VECTOR(7 downto 0)    -- 7-segment anodes
    );
end simon_says;

architecture Behavioral of simon_says is
    
    -- Состояния игры
    type state_type is (IDLE, SHOW_SEQUENCE, WAIT_INPUT, CHECK_INPUT, 
                        LEVEL_COMPLETE, GAME_OVER);
    signal current_state : state_type := IDLE;
    
    -- Константы времени
    constant LED_ON_TIME : integer := 50_000_000;   -- 0.5 сек
    constant LED_OFF_TIME : integer := 25_000_000;  -- 0.25 сек
    constant INPUT_TIMEOUT : integer := 500_000_000; -- 5 сек
    
    -- Счетчики
    signal clk_counter : integer := 0;
    signal timeout_counter : integer := 0;
    
    -- Переменные игры
    signal level : integer := 1;
    signal sequence_position : integer := 0;
    signal input_position : integer := 0;
    
    -- Последовательность
    type sequence_array is array (0 to 99) of integer;
    signal sequence : sequence_array := (others => 0);
    
    -- LFSR (RNG)
    signal lfsr : STD_LOGIC_VECTOR(15 downto 0) := "1010110011110001";
    
    -- Синхронизация переключателей (Debounce упрощенный)
    signal switches_prev : STD_LOGIC_VECTOR(15 downto 0) := (others => '0');
    signal switches_sync : STD_LOGIC_VECTOR(15 downto 0) := (others => '0');
    signal switch_pressed : STD_LOGIC_VECTOR(15 downto 0) := (others => '0');
    
    -- Таймер блокировки дребезга
    constant DEBOUNCE_TIME : integer := 5_000_000; 
    signal debounce_counter : integer := 0;
    signal input_locked : boolean := false;
    
    signal current_switch : integer := 0;
    
    -- Дисплей
    signal display_value : integer := 0;
    signal refresh_counter : integer := 0;
    signal digit_select : integer range 0 to 7 := 0;
    signal current_digit : integer := 0;
    
    -- Функция декодирования
    function digit_to_7seg(digit : integer) return STD_LOGIC_VECTOR is
    begin
        case digit is
            when 0 => return "0000001"; 
            when 1 => return "1001111"; 
            when 2 => return "0010010"; 
            when 3 => return "0000110"; 
            when 4 => return "1001100"; 
            when 5 => return "0100100"; 
            when 6 => return "0100000"; 
            when 7 => return "0001111"; 
            when 8 => return "0000000"; 
            when 9 => return "0000100"; 
            when others => return "1111111"; 
        end case;
    end function;
    
    -- Функция получения цифры
    function get_digit(value : integer; position : integer) return integer is
        variable temp : integer;
    begin
        temp := value;
        case position is
            when 0 => temp := temp / 1;
            when 1 => temp := temp / 10;
            when 2 => temp := temp / 100;
            when others => temp := 0;
        end case;
        return temp mod 10;
    end function;

begin

    process(clk, rst)
        variable random_led : integer := 0;
    begin
        if rst = '0' then 
            current_state <= IDLE;
            level <= 1;
            sequence_position <= 0;
            input_position <= 0;
            clk_counter <= 0;
            timeout_counter <= 0;
            leds <= (others => '0');
            lfsr <= "1010110011110001";
            current_switch <= 0;
            debounce_counter <= 0;
            input_locked <= false;
            
        elsif rising_edge(clk) then
            
            -- RNG
            lfsr <= lfsr(14 downto 0) & (lfsr(15) xor lfsr(13) xor lfsr(12) xor lfsr(10));
            
            -- Синхронизация
            switches_prev <= switches_sync;
            switches_sync <= switches;
            
            -- анти-дребезг
            if input_locked then
                if debounce_counter < DEBOUNCE_TIME then
                    debounce_counter <= debounce_counter + 1;
                    switch_pressed <= (others => '0'); -- Игнорируем ввод пока заблокировано
                else
                    input_locked <= false;
                    debounce_counter <= 0;
                end if;
            else
                for i in 0 to 15 loop
                    if switches_sync(i) = '0' and switches_prev(i) = '1' then
                        switch_pressed(i) <= '1';
                        input_locked <= true; -- Блокируем ввод на время дребезга
                    else
                        switch_pressed(i) <= '0';
                    end if;
                end loop;
            end if;
            
            
            case current_state is
                
                when IDLE =>
                    leds <= (others => '0');
                    display_value <= 0;
                    
                    if btn_start = '1' then
                        level <= 1;
                        sequence_position <= 0;
                        input_position <= 0;
                        
                        random_led := to_integer(unsigned(lfsr(3 downto 0)));
                        if random_led > 15 then random_led := random_led mod 16; end if;
                        sequence(0) <= random_led;
                        
                        current_state <= SHOW_SEQUENCE;
                        clk_counter <= 0;
                    end if;
                
                when SHOW_SEQUENCE =>
                    if clk_counter < LED_ON_TIME then
                        leds <= (others => '0');
                        if sequence(sequence_position) >= 0 and sequence(sequence_position) <= 15 then
                            leds(sequence(sequence_position)) <= '1';
                        end if;
                        clk_counter <= clk_counter + 1;
                    elsif clk_counter < LED_ON_TIME + LED_OFF_TIME then
                        leds <= (others => '0');
                        clk_counter <= clk_counter + 1;
                    else
                        clk_counter <= 0;
                        if sequence_position < level - 1 then
                            sequence_position <= sequence_position + 1;
                        else
                            sequence_position <= 0;
                            input_position <= 0;
                            timeout_counter <= 0;
                            current_state <= WAIT_INPUT;
                        end if;
                    end if;
                
                when WAIT_INPUT =>
                    leds <= (others => '0');
                    if timeout_counter < INPUT_TIMEOUT then
                        -- Оптимизация отображения, чтобы не делить часто
                        display_value <= (INPUT_TIMEOUT - timeout_counter) / 10000000;
                    else
                        display_value <= 0;
                    end if;
                    
                    if timeout_counter >= INPUT_TIMEOUT then
                        current_state <= GAME_OVER;
                        display_value <= 0;
                    else
                        timeout_counter <= timeout_counter + 1;
                        
                        for i in 0 to 15 loop
                            if switch_pressed(i) = '1' then
                                current_switch <= i;
                                leds(i) <= '1';
                                current_state <= CHECK_INPUT;
                                clk_counter <= 0;
                            end if;
                        end loop;
                    end if;
                
                when CHECK_INPUT =>
                    if clk_counter < LED_OFF_TIME then
                        clk_counter <= clk_counter + 1;
                    else
                        leds <= (others => '0');
                        if current_switch = sequence(input_position) then
                            if input_position < level - 1 then
                                input_position <= input_position + 1;
                                timeout_counter <= 0;
                                current_state <= WAIT_INPUT;
                            else
                                current_state <= LEVEL_COMPLETE;
                                clk_counter <= 0;
                            end if;
                        else
                            current_state <= GAME_OVER;
                        end if;
                    end if;
                
                when LEVEL_COMPLETE =>
                    display_value <= level;
                    if clk_counter < LED_ON_TIME then
                        leds <= (others => '1');
                        clk_counter <= clk_counter + 1;
                    elsif clk_counter < LED_ON_TIME * 2 then
                        leds <= (others => '0');
                        clk_counter <= clk_counter + 1;
                    else
                        level <= level + 1;
                        random_led := to_integer(unsigned(lfsr(3 downto 0)));
                        if random_led > 15 then random_led := random_led mod 16; end if;
                        
                        if level < 100 then
                            sequence(level) <= random_led;
                        end if;
                        
                        sequence_position <= 0;
                        input_position <= 0;
                        clk_counter <= 0;
                        current_state <= SHOW_SEQUENCE;
                    end if;
                
                when GAME_OVER =>
                    if clk_counter < LED_ON_TIME / 2 then
                        leds <= (others => '1');
                        clk_counter <= clk_counter + 1;
                    elsif clk_counter < LED_ON_TIME then
                        leds <= (others => '0');
                        clk_counter <= clk_counter + 1;
                    else
                        clk_counter <= 0;
                    end if;
                    
                    if level > 1 then display_value <= level - 1; else display_value <= 0; end if;
                    
                    if btn_start = '1' then
                        current_state <= IDLE;
                    end if;
            end case;
        end if;
    end process;
    
    -- Управление дисплеем
    process(clk)
    begin
        if rising_edge(clk) then
            if refresh_counter < 100_000 then
                refresh_counter <= refresh_counter + 1;
            else
                refresh_counter <= 0;
                if digit_select < 7 then
                    digit_select <= digit_select + 1;
                else
                    digit_select <= 0;
                end if;
            end if;
            
            an <= (others => '1');
            an(digit_select) <= '0';
            
            current_digit <= get_digit(display_value, digit_select);
            seg <= digit_to_7seg(current_digit);
            
            if digit_select = 1 and current_state /= IDLE then
                            dp <= '0';
                        else
                            dp <= '1';
                        end if;
        end if;
    end process;

end Behavioral;