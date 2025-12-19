----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 17:24:57
-- Design Name: 
-- Module Name: tb_REGn_async - Behavioral
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

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity tb_REGn_async is
end tb_REGn_async;

architecture Behavioral of tb_REGn_async is
    component REGn_async_beh
        generic (N : integer := 8);
        port (
            Din  : in std_logic_vector(N-1 downto 0);
            EN   : in std_logic;
            Dout : out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    component REGn_async_struct
        generic (N : integer := 8);
        port (
            Din  : in std_logic_vector(N-1 downto 0);
            EN   : in std_logic;
            Dout : out std_logic_vector(N-1 downto 0)
        );
    end component;

    constant N : integer := 4;
    
    signal Din   : std_logic_vector(N-1 downto 0) := (others => '0');
    signal EN    : std_logic := '0';
    signal Dout_beh, Dout_struct : std_logic_vector(N-1 downto 0);
    signal err : std_logic_vector(N-1 downto 0) := (others => '0');
begin
    -- Поведенческая модель
    uut_beh: REGn_async_beh 
        generic map (N => N)
        port map (
            Din => Din, 
            EN => EN, 
            Dout => Dout_beh
        );
    
    
    -- Структурная модель
    uut_struct : REGn_async_struct 
        generic map (N => N)
        port map (
            Din => Din, 
            EN => EN, 
            Dout => Dout_struct
        );
    
    -- Стимулы
    stim_proc : process
    begin
        -- Начальное состояние
        EN <= '0';
        Din <= "0000";
        wait for 10 ns;
        
        -- Разрешаем запись
        EN <= '1';
        Din <= "1010";
        wait for 10 ns;
        Din <= "1111";
        wait for 10 ns;
        
        -- Запрещаем запись - данные должны сохраниться
        EN <= '0';
        Din <= "0000";
        wait for 10 ns;
        
        -- Снова включаем запись
        EN <= '1';
        Din <= "0101";
        wait for 10 ns;
    end process;
    
    -- Проверка на равенство результатов
     err <= Dout_beh xor Dout_struct;
end Behavioral;
