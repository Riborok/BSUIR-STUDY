library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity lfsr_int is
    generic(
        N: integer;                    
        Polynom: std_logic_vector
    );
    port(
        CLK, RST: in std_logic;
        Q: out std_logic_vector(N-1 downto 0)
    );
end lfsr_int;

architecture Behavioral of lfsr_int is
    signal reg: std_logic_vector(N-1 downto 0) := (others => '1'); 
    signal next_reg: std_logic_vector(N-1 downto 0); 
begin
    process(reg)
        variable feedback_bit: std_logic;
    begin
        feedback_bit := reg(N - 1);
        
        for i in 1 to N-1 loop
            if Polynom(i) = '1' then
                next_reg(i) <= reg(i-1) xor feedback_bit;
            else
                next_reg(i) <= reg(i-1);
            end if;
        end loop;
        
        next_reg(0) <= feedback_bit;
    end process;
    
    process(CLK, RST)
    begin
        if RST = '1' then
            reg <= (others => '1'); 
        elsif rising_edge(CLK) then
            reg <= next_reg;
        end if;
    end process;
   
    Q <= reg;
end Behavioral;