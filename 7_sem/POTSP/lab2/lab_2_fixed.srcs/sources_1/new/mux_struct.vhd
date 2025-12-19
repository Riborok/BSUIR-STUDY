----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:48:05
-- Design Name: 
-- Module Name: mux_struct - Structural
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

entity mux_struct is
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           S : in STD_LOGIC;
           Z : out STD_LOGIC);
end mux_struct;


architecture Structural of mux_struct is


component and_2 is
    Port ( I1 : in STD_LOGIC;
           I2 : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

component or_2 is
    Port ( I1 : in STD_LOGIC;
           I2 : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

component inv is
    Port ( I : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

signal SN, ASN, SB: STD_LOGIC;

begin

INV_S: inv port map (S, SN);

AND_A_SN: and_2 port map (A, SN, ASN);
AND_S_B: and_2 port map (S, B, SB);

OR_Z_OUT: or_2 port map (ASN, SB, Z);

end Structural;
