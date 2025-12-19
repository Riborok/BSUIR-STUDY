library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
-- FTCLE (Flip-Flop Toggle/Clock Enable/Load/Clear)
entity DOP is
    Port (
        C   : in  STD_LOGIC;  -- Clock
        CLR : in  STD_LOGIC;  -- Asynchronous clear
        CE  : in  STD_LOGIC;  -- Clock enable
        L   : in  STD_LOGIC;  -- Load enable
        T   : in  STD_LOGIC;  -- Toggle enable
        D   : in  STD_LOGIC;  -- Data input
        Q   : out STD_LOGIC   -- Output
    );
end DOP;

architecture Behavioral of DOP is
    signal q_reg : STD_LOGIC := '0';
begin
    process (C, CLR)
    begin
        if CLR = '1' then
            q_reg <= '0';
        elsif rising_edge(C) then
            if CE = '1' then
                if L = '1' then
                    q_reg <= D;
                elsif T = '1' then
                    q_reg <= not q_reg;
                end if;
            end if;
        end if;
    end process;

    Q <= q_reg;
    
-- CLR = '1' ? сбрасывает Q сразу, независимо от такта.
-- rising_edge(CLK) ? обновление состояния происходит только при фронте такта.
-- CE = '1' ? разрешает реакцию на такт.
-- L = '1' ? загружает вход D.
-- T = '1' ? переключает текущее значение Q.
-- Если CE = '0' или L=T=0, состояние не меняется.
end Behavioral;
