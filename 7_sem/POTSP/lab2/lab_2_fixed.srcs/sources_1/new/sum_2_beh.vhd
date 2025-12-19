----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 17:44:30
-- Design Name: 
-- Module Name: sum_2_beh - Behavioral
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

entity sum_2_beh is
    Port ( A1 : in STD_LOGIC;
           B1 : in STD_LOGIC;
           A2 : in STD_LOGIC;
           B2 : in STD_LOGIC;
           CI : in STD_LOGIC;
           S1 : out STD_LOGIC;
           S2 : out STD_LOGIC;
           CO : out STD_LOGIC);
end sum_2_beh;

architecture Behavioral of sum_2_beh is

signal CO1 : std_logic;

begin

    S1 <= A1 xor B1 xor CI;
    
    CO1 <= (A1 and B1) or ((A1 xor B1) and CI);
    S2 <= A2 xor B2 xor CO1;
    
    CO <= (A2 and B2) or ((A2 xor B2) and CO1);

end Behavioral;
