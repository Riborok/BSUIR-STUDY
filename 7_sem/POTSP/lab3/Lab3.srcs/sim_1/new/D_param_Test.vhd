library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_param_Test is
--  Port ( );
end D_param_Test;

architecture Behavioral of D_param_Test is
 component D_param is
   generic (
       T_INV_DELAY: time;
       T_NOR_DELAY: time
   );
   Port (
       D: in STD_LOGIC;
       Q_inr, nQ_inr: out STD_LOGIC;
       Q_tps, nQ_tps: out STD_LOGIC
   );
   end component;
   
   signal D: STD_LOGIC := '0';
   signal Q_inr, nQ_inr, Q_tps, nQ_tps: STD_LOGIC;
   
   constant INV_DELAY_CONST: time := 4 ns;
   constant NOR_DELAY_CONST: time := 8 ns;
   constant GLITCH_TIME: time := 3 ns; 
begin
    uut: D_param
        generic map (
           T_INV_DELAY => INV_DELAY_CONST,
           T_NOR_DELAY => NOR_DELAY_CONST
        )
        port map (
           D       => D,
           Q_inr   => Q_inr,
           nQ_inr  => nQ_inr,
           Q_tps   => Q_tps,
           nQ_tps  => nQ_tps
        );  
    
    stim_proc: process
    begin
        D <= '1';
        wait for 30 ns;
        
        D <= '0';
        wait for 20 ns;
        
        D <= '1'; 
        wait for GLITCH_TIME;
        D <= '0';
        
        wait for 40 ns;
        
        report "The End";
        wait;
    end process;
end Behavioral;
