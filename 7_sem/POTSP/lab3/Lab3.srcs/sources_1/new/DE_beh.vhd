library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_beh is
  Port (
      D, E: in STD_LOGIC;
      Q, nQ: out STD_LOGIC
  );
end DE_beh;

architecture Behavioral of DE_beh is
  signal Q_int, nQ_int: STD_LOGIC;
begin
  nQ_int <= Q_int nor (E and D);
  Q_int <= nQ_int nor (E and not D); 
  
--  proc: process(E, D)
--  begin
--    if E = '1' then
--        Q_int <= D;
--    end if;
--  end process;
--  Q <= Q_int;
--  nQ <= not Q_int;
  
  Q <= Q_int;
  nQ <= nQ_int;
end Behavioral;
