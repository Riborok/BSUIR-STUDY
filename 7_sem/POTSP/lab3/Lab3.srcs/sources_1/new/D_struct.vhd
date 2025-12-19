library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity D_struct is
  Port ( 
    D: in STD_LOGIC;
    Q, nQ: out STD_LOGIC);
end D_struct;

architecture Structural of D_struct is
    component nor2_gate is 
        port (
            A, B: in STD_LOGIC;
            Z: out STD_LOGIC
        );
    end component;
    
    component not_gate is
        port (
            A: in STD_LOGIC;
            Z: out STD_LOGIC
        );
    end component;    
    
    signal Q_int, nQ_int, R: STD_LOGIC;
begin
    not1: not_gate port map (A => D, Z => R);
    nor21: nor2_gate port map (A => Q_int, B => D, Z => nQ_int);
    nor22: nor2_gate port map (A => nQ_int, B => R, Z => Q_int);
    
    Q <= Q_int;
    nQ <= nQ_int;

end Structural;
