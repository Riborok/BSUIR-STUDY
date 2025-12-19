library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity nor2_gate is
  Port ( 
    A, B: in STD_LOGIC;
    Z: out STD_LOGIC);
end nor2_gate;

architecture Structural of nor2_gate is
    component not_gate is
        Port (
            A: in STD_LOGIC;
            Z: out STD_LOGIC
        );
    end component;
    
    component or2_gate is
        Port (
            A, B: in STD_LOGIC;
            Z: out STD_LOGIC
        );
    end component;
    
    signal or21_out: STD_LOGIC;
begin
    or21: or2_gate port map (A => A, B => B, Z => or21_out);
    nor21: not_gate port map (A => or21_out, Z => Z);

end Structural;
