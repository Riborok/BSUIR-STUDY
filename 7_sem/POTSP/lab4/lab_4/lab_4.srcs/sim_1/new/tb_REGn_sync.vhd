----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 17:24:57
-- Design Name: 
-- Module Name: tb_REGn_sync - Behavioral
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

entity tb_REGn_sync is
end tb_REGn_sync;

architecture Behavioral of tb_REGn_sync is
    component REGn_sync_beh
        generic (N : integer := 8);
        port (
            Din  : in std_logic_vector(N-1 downto 0);
            EN   : in std_logic;
            CLK  : in  std_logic;
            Dout : out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    component REGn_sync_struct
        generic (N : integer := 8);
        port (
            Din  : in std_logic_vector(N-1 downto 0);
            EN   : in std_logic;
            CLK  : in  std_logic;
            Dout : out std_logic_vector(N-1 downto 0)
        );
    end component;

    constant N : integer := 4;
    
    signal Din   : std_logic_vector(N-1 downto 0) := (others => '0');
    signal CLK   : std_logic := '0';
    signal EN    : std_logic := '0';
    signal Dout_beh, Dout_struct : std_logic_vector(N-1 downto 0);
    signal err : std_logic_vector(N-1 downto 0) := (others => '0');
begin
    -- Поведенческая модель
    uut_beh: REGn_sync_beh 
        generic map (N => N)
        port map (
            Din => Din, 
            EN => EN, 
            CLK => CLK,
            Dout => Dout_beh
        );
    
    
    -- Структурная модель
    uut_struct : REGn_sync_struct 
        generic map (N => N)
        port map (
            Din => Din, 
            EN => EN, 
            CLK => CLK,
            Dout => Dout_struct
        );
    
  -- Тактовый сигнал
    clk_proc : process
    begin
        CLK <= '0'; wait for 5 ns;
        CLK <= '1'; wait for 5 ns;
    end process;
  
    -- Стимулы
    stim_proc : process
    begin   
        EN <= '0';
        Din <= "0000";
        wait for 20 ns;
        
        -- Разрешаем запись на фронтах
        EN <= '1';
        Din <= "1010"; wait for 20 ns;
        Din <= "1111"; wait for 20 ns;
        
        -- Отключаем запись
        EN <= '0';
        Din <= "0000"; wait for 20 ns;
        
        -- Снова включаем запись
        EN <= '1';
        Din <= "0101"; wait for 20 ns;
    end process;
  
    -- Проверка совпадения
     err <= Dout_beh xor Dout_struct;
end Behavioral;
