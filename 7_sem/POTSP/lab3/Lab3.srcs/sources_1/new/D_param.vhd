library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_param is
   generic (
       T_INV_DELAY : time := 2 ns;
       T_NOR_DELAY : time := 5 ns
   );
   Port (
       D         : in  STD_LOGIC;
       Q_inr, nQ_inr : out STD_LOGIC;
       Q_tps, nQ_tps : out STD_LOGIC
   );
end D_param;

architecture Behavioral of D_param is
    signal Q_inr_int, nQ_inr_int, R_inr_int : STD_LOGIC;
    signal Q_tps_int, nQ_tps_int, R_tps_int : STD_LOGIC;
begin
    R_inr_int <= not D after T_INV_DELAY;
    
    nQ_inr_int <= Q_inr_int nor D after T_NOR_DELAY;
    
    Q_inr_int <= nQ_inr_int nor R_inr_int after T_NOR_DELAY;

    Q_inr <= Q_inr_int;
    nQ_inr <= nQ_inr_int;

    R_tps_int <= transport not D after T_INV_DELAY;
    
    nQ_tps_int <= transport Q_tps_int nor D after T_NOR_DELAY;
    
    Q_tps_int <= transport nQ_tps_int nor R_tps_int after T_NOR_DELAY;
    
    Q_tps <= Q_tps_int;
    nQ_tps <= nQ_tps_int;
end Behavioral;
