----------------------------------------------------------------------------------
-- Company: 
-- Engineer: 
-- 
-- Create Date: 13.10.2025 14:19:41
-- Design Name: 
-- Module Name: tb_sum_1 - Behavioral
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

entity tb_sum_1 is
end tb_sum_1;

architecture Behavioral of tb_sum_1 is

component sum_1_beh
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           CI : in STD_LOGIC;
           S : out STD_LOGIC;
           CO : inout STD_LOGIC);
   
end component;

component sum_1_struct
    Port ( A : in STD_LOGIC;
           B : in STD_LOGIC;
           CI : in STD_LOGIC;
           S : out STD_LOGIC;
           CO : inout STD_LOGIC);
end component;

signal a: std_logic := '0';
signal b: std_logic := '0';
signal ci: std_logic := '0';

signal s_beh: std_logic;
signal s_struct: std_logic;

signal co_beh: std_logic;
signal co_struct: std_logic;

signal error_s: std_logic;
signal error_co: std_logic;
signal test_vector: std_logic_vector (2 downto 0);

constant period : time := 10 ns;

begin
    uut_beh: sum_1_beh port map (
        a => a,
        b => b,
        ci => ci,
        s => s_beh,
        co => co_beh
    );
    
    uut_struct: sum_1_struct port map (
        a => a,
        b => b,
        ci => ci,
        s => s_struct,
        co => co_struct
    );
    
    a <= test_vector(0);
    b <= test_vector(1);
    ci <= test_vector(2);
    
    stim_proc: process
    begin
    
        for i in 0 to 7 loop
            test_vector <= std_logic_vector(to_unsigned(i, test_vector'length));
            wait for period;
        end loop;  
        
        report "End of simulation" severity failure;
    
    end process;
    
    error_s <= s_beh xor s_struct;
    error_co <= co_beh xor co_struct;

end Behavioral;
