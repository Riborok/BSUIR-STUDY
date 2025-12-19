----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 15:54:53
-- Design Name: 
-- Module Name: demux_beh - Behavioral
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

entity demux_beh is
    Port ( A : in STD_LOGIC;
           S1 : in STD_LOGIC;
           S2 : in STD_LOGIC;
           Z1 : out STD_LOGIC;
           Z2 : out STD_LOGIC;
           Z3 : out STD_LOGIC;
           Z4 : out STD_LOGIC);
end demux_beh;

architecture Behavioral of demux_beh is

begin
    Z1 <= A when S1 = '0' and S2 = '0' else '0';
    Z2 <= A when S1 = '1' and S2 = '0' else '0';
    Z3 <= A when S1 = '0' and S2 = '1' else '0';
    Z4 <= A when S1 = '1' and S2 = '1' else '0';
end Behavioral;
