library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity bistab is
  Port (
    Q: out STD_LOGIC;
    nQ: out STD_LOGIC
  );
end bistab;

architecture Behavioral of bistab is
    signal Q_int  : STD_LOGIC := '0';
    signal nQ_int : STD_LOGIC := '1';
begin
    Q_int  <= not nQ_int;
    nQ_int <= not Q_int;
    
    Q <= Q_int;
    nQ <= nQ_int;

end Behavioral;
