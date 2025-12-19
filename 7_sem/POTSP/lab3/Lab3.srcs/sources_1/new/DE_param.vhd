library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_param is
    generic (
        T_INV_DELAY: time := 2 ns;
        T_AND_DELAY: time := 4 ns;
        T_NOR_DELAY: time := 5 ns
    );
    Port (
        D, E: in  STD_LOGIC;
        Q_inr, nQ_inr: out STD_LOGIC;
        Q_tps, nQ_tps: out STD_LOGIC
    );
end DE_param;

architecture Behavioral of DE_param is
   signal nD_inr, S_inr, R_inr         : STD_LOGIC;
   signal Q_inr_int, nQ_inr_int        : STD_LOGIC;
   
   signal nD_tps, S_tps, R_tps         : STD_LOGIC;
   signal Q_tps_int, nQ_tps_int        : STD_LOGIC;
begin
    nD_inr <= not D after T_INV_DELAY;
    S_inr  <= E and D after T_AND_DELAY;
    R_inr  <= E and nD_inr after T_AND_DELAY;
    
    nQ_inr_int <= Q_inr_int nor S_inr after T_NOR_DELAY;
    Q_inr_int  <= nQ_inr_int nor R_inr after T_NOR_DELAY;
    
    Q_inr <= Q_inr_int;
    nQ_inr <= nQ_inr_int;  
    
    nD_tps <= transport not D after T_INV_DELAY;
    S_tps  <= transport E and D after T_AND_DELAY;
    R_tps  <= transport E and nD_tps after T_AND_DELAY;
    
    nQ_tps_int <= transport Q_tps_int nor S_tps after T_NOR_DELAY;
    Q_tps_int  <= transport nQ_tps_int nor R_tps after T_NOR_DELAY;
    
    Q_tps <= Q_tps_int;
    nQ_tps <= nQ_tps_int; 
end Behavioral;
