----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 12:55:12
-- Design Name: 
-- Module Name: tb_demux - Behavioral
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

entity tb_demux is
end tb_demux;

architecture Behavioral of tb_demux is

component demux_beh
    port (
        A: in std_logic;
        S1: in std_logic;
        S2: in std_logic;
        Z1: out std_logic;
        Z2: out std_logic;
        Z3: out std_logic;
        Z4: out std_logic);
end component;

component demux_struct
    port (
        A: in std_logic;
        S1: in std_logic;
        S2: in std_logic;
        Z1: out std_logic;
        Z2: out std_logic;
        Z3: out std_logic;
        Z4: out std_logic);
end component;

signal a: std_logic := '0';
signal s1: std_logic := '0';
signal s2: std_logic := '0';

signal z1_beh: std_logic;
signal z2_beh: std_logic;
signal z3_beh: std_logic;
signal z4_beh: std_logic;
signal z1_struct: std_logic;
signal z2_struct: std_logic;
signal z3_struct: std_logic;
signal z4_struct: std_logic;

signal error_z1: std_logic;
signal error_z2: std_logic;
signal error_z3: std_logic;
signal error_z4: std_logic;

constant period : time := 10 ns;

begin
    uut_beh: demux_beh PORT MAP (
        a => a,
        s1 => s1,
        s2 => s2,
        z1 => z1_beh,
        z2 => z2_beh,
        z3 => z3_beh,
        z4 => z4_beh
    );
    
    uut_struct: demux_struct PORT MAP (
        a => a,
        s1 => s1,
        s2 => s2,
        z1 => z1_struct,
        z2 => z2_struct,
        z3 => z3_struct,
        z4 => z4_struct
    );
    
    a <= not a after period;
    s1 <= not s1 after period*2;
    s2 <= not s2 after period*4;
    
    error_z1 <= z1_beh xor z1_struct;
    error_z2 <= z2_beh xor z2_struct;
    error_z3 <= z3_beh xor z3_struct;
    error_z4 <= z4_beh xor z4_struct;

end Behavioral;
