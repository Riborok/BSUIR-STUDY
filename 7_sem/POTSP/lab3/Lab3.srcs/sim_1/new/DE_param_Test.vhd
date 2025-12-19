library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_param_Test is
--  Port ( );
end DE_param_Test;

architecture Behavioral of DE_param_Test is
    component DE_param is
        generic (
            T_INV_DELAY: time;
            T_AND_DELAY: time;
            T_NOR_DELAY: time
        );
        Port (
            D, E: in  STD_LOGIC;
            Q_inr, nQ_inr: out STD_LOGIC;
            Q_tps, nQ_tps: out STD_LOGIC
        );
    end component;
    
    signal D, E: STD_LOGIC := '0';
    signal Q_inr, nQ_inr, Q_tps, nQ_tps: STD_LOGIC;

    constant INV_DELAY_CONST: time := 3 ns;
    constant AND_DELAY_CONST: time := 5 ns;
    constant NOR_DELAY_CONST: time := 6 ns;
    constant GLITCH_TIME: time := 2 ns; 
begin
    uut: DE_param
        generic map (
            T_INV_DELAY => INV_DELAY_CONST,
            T_AND_DELAY => AND_DELAY_CONST,
            T_NOR_DELAY => NOR_DELAY_CONST
        )
        port map (
            D => D, E => D,
            Q_inr => Q_inr, nQ_inr => nQ_inr,
            Q_tps => Q_tps, nQ_tps => nQ_tps
        );
        
    stim_proc: process
    begin
        wait for 10 ns;
        
        E <= '1'; 
        wait for 10 ns;
        D <= '1';
        wait for 20 ns;
        D <= '0'; 
        wait for 20 ns;
        
        D <= '1'; 
        wait for 15 ns;
        E <= '0';
        wait for 10 ns;
        D <= '0';
        wait for 20 ns;
        
        E <= '1';
        wait for 15 ns;
        
        D <= '1';
        wait for GLITCH_TIME;
        D <= '0';
        
        wait for 30 ns;
        
        report "The end" severity failure;
        wait;
    end process;
end Behavioral;