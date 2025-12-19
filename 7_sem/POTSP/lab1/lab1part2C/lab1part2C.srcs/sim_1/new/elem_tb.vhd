library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.NUMERIC_STD.ALL; 

entity elem_tb is
end elem_tb;

architecture Behavioral of elem_tb is

    component elem
        Port ( A : in STD_LOGIC;
               B : in STD_LOGIC;
               C : in STD_LOGIC;
               Q : out STD_LOGIC;
               nQ : out STD_LOGIC);
    end component;

    signal s_A, s_B, s_C : std_logic;
    signal s_Q, s_nQ     : std_logic;

    constant period : time := 10 ns;

begin

    UUT: elem port map (
        A  => s_A,
        B  => s_B,
        C  => s_C,
        Q  => s_Q,
        nQ => s_nQ
    );

    stimulus_process: process
        variable i_vec : std_logic_vector(2 downto 0);
    begin
        report "Starting simulation: iterating through all 8 input combinations...";

        for i in 0 to 7 loop
            wait for period;
            i_vec := std_logic_vector(to_unsigned(i, 3));
            
            s_A <= i_vec(2);
            s_B <= i_vec(1);
            s_C <= i_vec(0); 

        end loop;

        report "Simulation finished.";
        wait;
    end process;

end Behavioral;