----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 17:24:57
-- Design Name: 
-- Module Name: tb_counter_FSM - Behavioral
-- Project Name: 
-- Target Devices: 
-- Tool Versions: 
-- Description: 
-- 
-- Dependencies: 
-- 
-- Revision:
-- Revision 0.01 - File Created
-- Additional Comments:
-- 
----------------------------------------------------------------------------------


library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity tb_counter_FSM is
end tb_counter_FSM;

architecture Behavioral of tb_counter_FSM is
    signal CLK  : std_logic := '0';
    signal RST  : std_logic := '0';
    signal COUNT : std_logic_vector(3 downto 0);

    component counter_FSM
        port (
            CLK : in std_logic;
            RST : in std_logic;
            COUNT : out std_logic_vector(3 downto 0)
        );
    end component;
begin
    -- Инстанцирование счётчика
    uut : counter_FSM
        port map (
            CLK => CLK,
            RST => RST,
            COUNT => COUNT
        );

    -- Генератор тактового сигнала
    clk_gen : process
    begin
        CLK <= '0'; wait for 5 ns;
        CLK <= '1'; wait for 5 ns;
    end process;

    -- Стимулы
    stim_proc : process
    begin
        -- Сброс
        RST <= '1'; wait for 10 ns;
        RST <= '0';

        -- Наблюдаем 20 тактов
        wait for 200 ns;

        -- Повторный сброс
        RST <= '1'; wait for 10 ns;
        RST <= '0';
        wait for 100 ns;
    end process;
end Behavioral;
