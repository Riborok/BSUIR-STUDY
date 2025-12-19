library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity DE_struct is
  Port (
    D, E: in STD_LOGIC;
    Q, nQ: out STD_LOGIC );
end DE_struct;

architecture Structural of DE_struct is
    component nor2_gate is
        Port (
            A, B: in STD_LOGIC;
            Z: out STD_LOGIC);
    end component;
    
    component and2_gate is
        Port (
            A, B: in STD_LOGIC;
            Z: out STD_LOGIC);
    end component;
    
    component not_gate is
        Port (
            A: in STD_LOGIC;
            Z: out STD_LOGIC);
    end component;
    
    signal Q_int, nQ_int, S, R, nD: STD_LOGIC;
begin
    not1: not_gate port map (A => D, Z => nD);
    and21: and2_gate port map (A => E, B => D, Z => S);
    and22: and2_gate port map (A => E, B => nD, Z => R);
    nor21: nor2_gate port map (A => Q_int, B => S, Z => nQ_int);
    nor22: nor2_gate port map (A => nQ_int, B => R, Z => Q_int);
    
    Q <= Q_int;
    nQ <= nQ_int;

end Structural;
