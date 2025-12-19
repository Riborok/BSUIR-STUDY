----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 16:39:28
-- Design Name: 
-- Module Name: and_5_struct - Structural
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

entity and_5_struct is
    Port (
        I : in  STD_LOGIC_VECTOR (4 downto 0);
        O : out STD_LOGIC
    );
end and_5_struct;

architecture Structural of and_5_struct is

component and_2 is
    Port (
        I1 : in  STD_LOGIC;
        I2 : in  STD_LOGIC;
        O  : out STD_LOGIC
    );
end component;

signal and_temp : std_logic_vector(3 downto 0);

begin

    and_first: and_2 port map (
        I1 => I(0),
        I2 => I(1),
        O  => and_temp(0)
    );
    
    gen_and: for j in 1 to 3 generate
        and_gate: and_2 port map (
            I1 => and_temp(j-1),
            I2 => I(j+1),
            O  => and_temp(j)
        );
    end generate;

    O <= and_temp(3);

end Structural;
