----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 16:56:13
-- Design Name: 
-- Module Name: REGn_sync_beh - Behavioral
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

entity REGn_sync_beh is
    generic (N : integer := 8);
    port (
        Din  : in  std_logic_vector(N-1 downto 0);
        CLK  : in  std_logic;
        EN   : in  std_logic;
        Dout : out std_logic_vector(N-1 downto 0)
    );
end REGn_sync_beh;

architecture Behavioral of REGn_sync_beh is
    signal reg_data : std_logic_vector(N-1 downto 0);
begin
    process(CLK)
    begin
        if rising_edge(CLK) then
            if EN = '1' then
                reg_data <= Din;
            end if;
        end if;
    end process;
    
    Dout <= reg_data;
end Behavioral;
