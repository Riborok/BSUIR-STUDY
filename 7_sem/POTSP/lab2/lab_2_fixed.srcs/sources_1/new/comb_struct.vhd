----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 15:35:20
-- Design Name: 
-- Module Name: comb_struct - Structural
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

entity comb_struct is
    Port ( X : in STD_LOGIC;
           Y : in STD_LOGIC;
           Z : in STD_LOGIC;
           F : out STD_LOGIC);
end comb_struct;

architecture Structural of comb_struct is

component and_2 is
    Port ( I1 : in STD_LOGIC;
           I2 : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

component and_3 is
    Port ( I1 : in STD_LOGIC;
           I2 : in STD_LOGIC;
           I3 : in STD_LOGIC;
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

signal XN, YN, ZN, X_YN, X_YN_Z, XN_Y_ZN: STD_LOGIC;

begin

    INV_X: inv port map (X, XN);
    INV_Y: inv port map (Y, YN);
    INV_Z: inv port map (Z, ZN);
    
    OR_X_YN: or_2 port map (X, YN, X_YN);
    
    AND_XYN_Z: and_2 port map (X_YN, Z, X_YN_Z);
    AND_XN_Y_ZN: and_3 port map (XN, Y, ZN, XN_Y_ZN);
    
    OR_OUT: or_2 port map (X_YN_Z, XN_Y_ZN, F);

end Structural;
