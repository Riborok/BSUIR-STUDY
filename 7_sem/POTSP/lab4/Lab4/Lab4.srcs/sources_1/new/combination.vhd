library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity combination is
  Port ( 
    Din: in std_logic_vector(6 downto 0);
    Dout: out std_logic
  );
end combination;

architecture Behavioral of combination is
   signal int_signal: std_logic;
begin
   int_signal <= Din(0) xor Din(2) xor Din(4);
   Dout <= (Din(1) xor Din(3) xor Din(5)) and int_signal;
   
end Behavioral;
