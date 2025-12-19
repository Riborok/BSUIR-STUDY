----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 09.11.2025 19:18:34
-- Design Name: 
-- Module Name: SREGn_beh - Behavioral
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

entity SREGn_beh is
generic (N : integer := 8);
    port (
        Sin  : in std_logic;
        SE   : in std_logic;
        CLK  : in std_logic;
        RST  : in std_logic;
        Pout : out std_logic_vector(N-1 downto 0)
    );
end SREGn_beh;

architecture Behavioral of SREGn_beh is
    signal reg_data : std_logic_vector(N-1 downto 0);
begin
    process(CLK, RST)
    begin
        if RST = '1' then
            reg_data <= (others => '0');
        elsif rising_edge(CLK) then
            if SE = '1' then
                reg_data <= reg_data(N-2 downto 0) & Sin;
            end if;
        end if;
    end process;
    
    Pout <= reg_data;
end Behavioral;
