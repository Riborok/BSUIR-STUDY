library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity RS_param_Test is
--  Port ( );
end RS_param_Test;

architecture Behavioral of RS_param_Test is
    component RS_param is
        generic (
            T_INERTIAL: time := 10 ns;
            T_TRANSPORT: time := 10 ns
        );
        Port (
            S, R: in STD_LOGIC;
            Q_inr, nQ_inr: out STD_LOGIC;
            Q_tps, nQ_tps: out STD_LOGIC
        );
    end component;
    
    signal S, R: STD_LOGIC := '0';
    signal Q_inr, nQ_inr: STD_LOGIC;
    signal Q_tps, nQ_tps: STD_LOGIC;
    
    constant INERTIAL_D  : time := 15 ns;
    constant TRANSPORT_D : time := 5 ns;
begin
    uut: RS_param
        generic map (
            T_INERTIAL  => INERTIAL_D,
            T_TRANSPORT => TRANSPORT_D
        )
        port map (
            S => S,
            R => R,
            Q_inr => Q_inr,
            nQ_inr => nQ_inr,
            Q_tps => Q_tps,
            nQ_tps => nQ_tps
        );
    
    stim_proc: process
    begin
        S <= '1';
        wait for 20 ns;
        S <= '0';
        wait for 20 ns;
        
        R <= '1';
        wait for 5 ns; 
        R <= '0';
        
        wait for 40 ns;
    end process;
end Behavioral;
