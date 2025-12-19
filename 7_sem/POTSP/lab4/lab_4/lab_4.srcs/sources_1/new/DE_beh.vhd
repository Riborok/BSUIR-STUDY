library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_latch is
    port (
        Din, E: in std_logic;
        Dout: out std_logic
    );
end DE_latch;

architecture Behavioral of DE_latch is
    signal Q_int, nQ_int: std_logic;
begin
    nQ_int <= Q_int nor (E and Din);
    Q_int <= nQ_int nor (E and not Din); 
    
    Dout <= Q_int;
end Behavioral;
