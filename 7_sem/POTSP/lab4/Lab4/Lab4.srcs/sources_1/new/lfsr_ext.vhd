library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity lfsr_ext is
    generic(
        N: integer;                    
        Polynom: std_logic_vector
    );
    port(
        CLK, RST: in std_logic;
        Q: out std_logic_vector(N-1 downto 0)
    );
end lfsr_ext;

architecture Behavioral of lfsr_ext is
    signal reg: std_logic_vector(N-1 downto 0) := (others => '1'); 
    signal feedback: std_logic; 
begin
    process(reg)
       variable temp: std_logic;
    begin
       temp := '0';
       for i in 0 to N-1 loop
           if Polynom(i) = '1' then
               temp := temp xor reg(i);
           end if;
       end loop;
       feedback <= temp;
    end process;
    
    process(CLK, RST)
    begin
       if RST = '1' then
           reg <= (others => '1'); 
       elsif rising_edge(CLK) then
           reg <= feedback & reg(N-1 downto 1);
       end if;
    end process;
    
    Q <= reg;
end Behavioral;
