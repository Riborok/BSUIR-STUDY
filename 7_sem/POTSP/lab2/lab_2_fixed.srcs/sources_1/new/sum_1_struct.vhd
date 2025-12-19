----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 11:57:36
-- Design Name: 
-- Module Name: sum_1_struct - Structural
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

entity sum_1_struct is
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           CI : in STD_LOGIC;
           S : out STD_LOGIC;
           CO : inout STD_LOGIC);
end sum_1_struct;

architecture Structural of sum_1_struct is

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

component or_3 is
    Port ( I1 : in STD_LOGIC;
           I2 : in STD_LOGIC;
           I3 : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

component inv 
    Port ( I : in STD_LOGIC;
           O : out STD_LOGIC);
end component;

signal A_B, A_CI, B_CI : STD_LOGIC;
signal A_B_CI_OR, A_B_CI_AND, CON, ABCI_CO : STD_LOGIC;

begin

AND_A_B: and_2 port map (A, B, A_B);
AND_A_CI: and_2 port map (A, CI, A_CI);
AND_B_CI: and_2 port map (B, CI, B_CI);

OR_P_OUT: or_3 port map (A_B, A_CI, B_CI, CO);


OR_A_B_CI: or_3 port map (A, B, CI, A_B_CI_OR);
AND_A_B_CI: and_3 port map (A, B, CI, A_B_CI_AND);

INV_CO: inv port map (CO, CON);
AND_ABCI_CO: and_2 port map (A_B_CI_OR, CON, ABCI_CO);

OR_S_OUT: or_2 port map (ABCI_CO, A_B_CI_AND, S);

end Structural;
