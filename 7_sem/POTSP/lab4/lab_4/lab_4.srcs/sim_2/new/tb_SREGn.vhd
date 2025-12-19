----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 17:24:57
-- Design Name: 
-- Module Name: tb_SREGn_sync - Behavioral
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

entity tb_SREGn is
end tb_SREGn;

architecture Behavioral of tb_SREGn is
    component SREGn_beh
        generic (N : integer := 8);
        port (
            Sin  : in std_logic;
            SE   : in std_logic;
            CLK  : in std_logic;
            RST  : in std_logic;
            Pout : out std_logic_vector(N-1 downto 0)
        );
    end component;

    component SREGn_struct
        generic (N : integer := 8);
        port (
            Sin  : in std_logic;
            SE   : in std_logic;
            CLK  : in std_logic;
            RST  : in std_logic;
            Pout : out std_logic_vector(N-1 downto 0)
        );
    end component;

    constant N : integer := 4;

    signal Sin, SE, CLK, RST : std_logic := '0';
    signal Pout_beh, Pout_struct : std_logic_vector(N-1 downto 0);
    signal err : std_logic_vector(N-1 downto 0);
begin
    -- Поведенческая модель
    uut_beh : SREGn_beh
        generic map (N => N)
        port map (
            Sin => Sin,
            SE => SE,
            CLK => CLK,
            RST => RST,
            Pout => Pout_beh
        );

    -- Структурная модель
    uut_struct : SREGn_struct
        generic map (N => N)
        port map (
            Sin => Sin,
            SE => SE,
            CLK => CLK,
            RST => RST,
            Pout => Pout_struct
        );

    -- Генерация тактового сигнала
    clk_proc : process
    begin
        CLK <= '0'; wait for 5 ns;
        CLK <= '1'; wait for 5 ns;
    end process;

    -- Стимулы
    stim_proc : process
    begin
        RST <= '1'; wait for 10 ns;
        RST <= '0';
        SE  <= '1';
        
        -- Подать биты на вход Sin
        Sin <= '1'; wait for 10 ns;
        Sin <= '0'; wait for 10 ns;
        Sin <= '1'; wait for 10 ns;
        Sin <= '1'; wait for 10 ns;
        
        -- Остановить сдвиг
        SE <= '0';
        Sin <= '0';
        wait for 20 ns;
    end process;

    -- Ошибка, если выходы не совпадают
    err <= Pout_beh xor Pout_struct;
end Behavioral;
