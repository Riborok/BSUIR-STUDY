library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity RS_beh is
 Port (
    S, R: in STD_LOGIC;
    Q, nQ: out STD_LOGIC
 );
end RS_beh;

architecture Behavioral of RS_beh is
    signal Q_int: STD_LOGIC := '0';
    signal nQ_int: STD_LOGIC := '1';
begin
    nQ_int <= Q_int nor S;
    Q_int <= nQ_int nor R;
    Q <= Q_int;
    nQ <= nQ_int;

end Behavioral;
