library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.NUMERIC_STD.ALL;

entity mul_tb is
end mul_tb;

architecture Behavioral of mul_tb is

    component mul
        Port ( A1 : in STD_LOGIC;
               A2 : in STD_LOGIC;
               B1 : in STD_LOGIC;
               B2 : in STD_LOGIC;
               S  : in STD_LOGIC;
               Q1 : out STD_LOGIC;
               Q2 : out STD_LOGIC);
    end component;

    signal s_A1, s_A2, s_B1, s_B2, s_S : std_logic;
    signal s_Q1, s_Q2                 : std_logic;

    constant period : time := 10 ns;

begin

    UUT: mul port map (
        A1 => s_A1,
        A2 => s_A2,
        B1 => s_B1,
        B2 => s_B2,
        S  => s_S,
        Q1 => s_Q1,
        Q2 => s_Q2
    );

    stimulus_process: process
        variable i_vec : std_logic_vector(4 downto 0);
    begin
        report "Starting simulation: iterating through all 32 input combinations...";

        
        for i in 0 to 31 loop
            wait for period;
            i_vec := std_logic_vector(to_unsigned(i, 5));
            
            s_S  <= i_vec(4);
            s_B2 <= i_vec(3);
            s_B1 <= i_vec(2);
            s_A2 <= i_vec(1);
            s_A1 <= i_vec(0); 
               
        end loop;
        wait;
    end process;

end Behavioral;