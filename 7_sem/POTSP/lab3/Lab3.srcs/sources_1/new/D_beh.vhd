library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_beh is
  Port (
    D: in STD_LOGIC;
    Q, nQ: out STD_LOGIC
  );
end D_beh;

architecture Behavioral of D_beh is
  signal Q_int, nQ_int: STD_LOGIC;
begin
  nQ_int <= Q_int nor D;
  Q_int <= nQ_int nor not D;
  
  Q <= Q_int;
  nQ <= nQ_int;
end Behavioral;
