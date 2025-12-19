library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity lfsr_w_2_dir is
    generic(
        N: integer := 5;                    
        Polynom: std_logic_vector := "00101"
    );
    port(
        CLK, RST: in std_logic;
        MODE, LOAD: in std_logic;                                      
        BUS_DATA: inout std_logic_vector(N-1 downto 0)
    );
end lfsr_w_2_dir;

architecture Behavioral of lfsr_w_2_dir is
    signal reg: std_logic_vector(N-1 downto 0) := (others => '1'); 
    signal feedback: std_logic; 
begin
    feedback_calc: process(reg)
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
    
    lfsr_proc: process(CLK, RST)
    begin
       if RST = '1' then
           reg <= (others => '1'); 
       elsif rising_edge(CLK) then
           if LOAD = '1' then
              if MODE = '1' then
                  reg <= BUS_DATA;
              end if;
           elsif MODE = '0' then
               reg <= feedback & reg(N-1 downto 1);
           end if;
       end if;
    end process;
    
    BUS_DATA <= reg when MODE = '0' else (others => 'Z');

end Behavioral;
