----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 16:39:28
-- Design Name: 
-- Module Name: and_5_beh - Behavioral
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

entity and_5_beh is
    Port ( I : in STD_LOGIC_VECTOR (4 downto 0);
           O : out STD_LOGIC);
end and_5_beh;

architecture Behavioral of and_5_beh is

begin
    O <= I(0) and I(1) and I(2) and I(3) and I(4);
end Behavioral;
