----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:59:55
-- Design Name: 
-- Module Name: mux_4_2_struct - Structural
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

entity mux_4_2_struct is
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           A1 : in STD_LOGIC;
           B1 : in STD_LOGIC;
           S : in STD_LOGIC;
           Z : out STD_LOGIC;
           Z1 : out STD_LOGIC);
end mux_4_2_struct;

architecture Structural of mux_4_2_struct is

component mux_struct is
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           S : in STD_LOGIC;
           Z : out STD_LOGIC);
end component;
           
begin

MUX_1: mux_struct port map (A, B, S, Z);
MUX_2: mux_struct port map (A1, B1, S, Z1);

end Structural;
