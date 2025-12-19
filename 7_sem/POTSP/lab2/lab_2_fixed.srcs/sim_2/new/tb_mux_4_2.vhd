----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:55:12
-- Design Name: 
-- Module Name: tb_mux_4_2 - Behavioral
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
use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity tb_mux_4_2 is
end tb_mux_4_2;

architecture Behavioral of tb_mux_4_2 is

component mux_4_2_beh
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           A1 : in STD_LOGIC;
           B1 : in STD_LOGIC;
           S : in STD_LOGIC;
           Z : out STD_LOGIC;
           Z1 : out STD_LOGIC);
end component;

component mux_4_2_struct
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           A1 : in STD_LOGIC;
           B1 : in STD_LOGIC;
           S : in STD_LOGIC;
           Z : out STD_LOGIC;
           Z1 : out STD_LOGIC);
end component;

signal a: std_logic := '0';
signal b: std_logic := '0';
signal a1: std_logic := '0';
signal b1: std_logic := '0';
signal s: std_logic := '0';

signal z_beh: std_logic;
signal z_struct: std_logic;

signal z1_beh: std_logic;
signal z1_struct: std_logic;

signal error_z: std_logic;
signal error_z1: std_logic;
signal test_vector: std_logic_vector (4 downto 0);

constant period : time := 10 ns;

begin
    uut_beh: mux_4_2_beh port map (
        a => a,
        b => b,
        a1 => a1,
        b1 => b1,
        s => s,
        z => z_beh,
        z1 => z1_beh
    );
    
    uut_struct: mux_4_2_struct port map (
        a => a,
        b => b,
        a1 => a1,
        b1 => b1,
        s => s,
        z => z_struct,
        z1 => z1_struct
    );
    
    s <= test_vector(0);
    a <= test_vector(1);
    b <= test_vector(2);
    a1 <= test_vector(3);
    b1 <= test_vector(4);
    
    stim_proc: process
    begin
    
        for i in 0 to 31 loop
            test_vector <= std_logic_vector(to_unsigned(i, test_vector'length));
            wait for period;
        end loop;  
        
        report "End of simulation" severity failure;
    
    end process;
    
    error_z <= z_beh xor z_struct;
    error_z1 <= z1_beh xor z1_struct;

end Behavioral;
