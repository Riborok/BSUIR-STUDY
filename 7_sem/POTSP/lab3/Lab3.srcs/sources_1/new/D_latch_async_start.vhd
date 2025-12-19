library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_latch_async_start is
  Port ( 
      D, E, Pre: in STD_LOGIC;
      Q, nQ: out STD_LOGIC);
end D_latch_async_start;

architecture Behavioral of D_latch_async_start is
    signal Q_int: STD_LOGIC;
begin
    proc: process(D, E, Pre)
    begin
        if Pre = '1' then
            Q_int <= '1';
        elsif E = '1' then
            Q_int <= D;    
        end if;
    end process;
    
    Q <= Q_int;
    nQ <= not Q_int;
end Behavioral;
