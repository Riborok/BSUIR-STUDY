library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

entity DOP_Test is
end DOP_Test;

architecture Behavioral of DOP_Test is
    -- Компонент для тестирования
    component DOP is
        Port (
            C   : in  STD_LOGIC;  -- Clock
            CLR : in  STD_LOGIC;  -- Asynchronous clear
            CE  : in  STD_LOGIC;  -- Clock enable
            L   : in  STD_LOGIC;  -- Load enable
            T   : in  STD_LOGIC;  -- Toggle enable
            D   : in  STD_LOGIC;  -- Data input
            Q   : out STD_LOGIC   -- Output
        );
    end component;
    
    -- Сигналы для соединения
    signal C, CLR, CE, L, T, D, Q : STD_LOGIC := '0';
    
    -- Константы
    constant CLK_PERIOD : time := 10 ns;
    
begin
    -- Инстанцирование тестируемого модуля
    uut: DOP
        port map (
            C   => C,
            CLR => CLR,
            CE  => CE,
            L   => L,
            T   => T,
            D   => D,
            Q   => Q
        );
    
    -----------------------------------------------------
    -- Генератор тактового сигнала
    -----------------------------------------------------
    clk_process : process
    begin
        while TRUE loop
            C <= '0';
            wait for CLK_PERIOD / 2;
            C <= '1';
            wait for CLK_PERIOD / 2;
        end loop;
    end process;
    
    -----------------------------------------------------
    -- Основной тестовый процесс
    -----------------------------------------------------
    stim_proc : process
    begin
        report "=== Начало теста DOP (FTCLE) ===";
        
        -- 1. Асинхронный сброс
        CLR <= '1';
        wait for 5 ns;
        CLR <= '0';
        wait for CLK_PERIOD;
        
        -- 2. Проверка загрузки (Load)
        CE <= '1';
        L  <= '1';
        D  <= '1';
        wait for CLK_PERIOD;
        L  <= '0';
        wait for CLK_PERIOD;
        
        -- 3. Проверка переключения (Toggle)
        T <= '1';
        wait for CLK_PERIOD;
        T <= '0';
        wait for CLK_PERIOD;
        T <= '1';
        wait for CLK_PERIOD;
        T <= '0';
        wait for CLK_PERIOD;
        
        -- 4. Проверка блокировки CE
        CE <= '0';     -- запрещаем такт
        L  <= '1';
        D  <= '0';
        wait for 2 * CLK_PERIOD;
        
        -- 5. Снова разрешаем CE, загружаем 0
        CE <= '1';
        L  <= '1';
        D  <= '0';
        wait for CLK_PERIOD;
        L  <= '0';
        wait for CLK_PERIOD;
        
        -- 6. Проверка CLR во время CE=1
        CLR <= '1';
        wait for 5 ns;
        CLR <= '0';
        wait for CLK_PERIOD;
        
        report "=== Конец теста DOP ===";
        wait;
    end process;

end Behavioral;
