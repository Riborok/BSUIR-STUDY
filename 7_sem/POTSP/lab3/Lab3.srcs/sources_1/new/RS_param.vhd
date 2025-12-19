library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity RS_param is
   generic (
       T_INERTIAL: time := 10 ns;
       T_TRANSPORT: time := 10 ns
   );
   Port (
       S, R: in STD_LOGIC;
       Q_inr, nQ_inr: out STD_LOGIC;
       Q_tps, nQ_tps: out STD_LOGIC
   );
end RS_param;

architecture Behavioral of RS_param is
    signal Q_inr_int, nQ_inr_int: STD_LOGIC;
    signal Q_tps_int, nQ_tps_int: STD_LOGIC;
begin
    nQ_inr_int <= Q_inr_int nor S after T_INERTIAL;
    Q_inr_int <= nQ_inr_int nor R after T_INERTIAL;
    
    Q_inr <= Q_inr_int;
    nQ_inr <= nQ_inr_int;

    nQ_tps_int <= transport Q_tps_int nor S after T_TRANSPORT;
    Q_tps_int <= transport nQ_tps_int nor R after T_TRANSPORT;

    Q_tps <= Q_tps_int;
    nQ_tps <= nQ_tps_int;
end Behavioral;
