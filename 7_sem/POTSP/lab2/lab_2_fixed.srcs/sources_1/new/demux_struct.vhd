----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 15:54:53
-- Design Name: 
-- Module Name: demux_struct - Structural
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

entity demux_struct is
    Port ( A : in STD_LOGIC;
           S1 : in STD_LOGIC;
           S2 : in STD_LOGIC;
           Z1 : out STD_LOGIC;
           Z2 : out STD_LOGIC;
           Z3 : out STD_LOGIC;
           Z4 : out STD_LOGIC);
end demux_struct;

architecture Structural of demux_struct is

component and_3 is
    Port ( I1 : in STD_LOGIC;
           I2 : in STD_LOGIC;
           I3 : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

component inv is
    Port ( I : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

signal S1N, S2N : STD_LOGIC;

begin
    INV_S1: inv port map (S1, S1N);
    INV_S2: inv port map (S2, S2N);
    
    AND_Z1_OUT: and_3 port map (S1N, S2N, A, Z1);
    AND_Z2_OUT: and_3 port map (S1, S2N, A, Z2);
    AND_Z3_OUT: and_3 port map (S1N, S2, A, Z3);
    AND_Z4_OUT: and_3 port map (S1, S2, A, Z4);
end Structural;
