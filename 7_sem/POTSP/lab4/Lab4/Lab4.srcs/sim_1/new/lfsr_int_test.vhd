library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity lfsr_int_test is
end lfsr_int_test;

architecture Behavioral of lfsr_int_test is
    constant N: integer := 5;
    constant Polynom: std_logic_vector(N-1 downto 0) := "00101";
    constant DELAY: time := 10 ns;
    
    component lfsr_int is
        generic (
             N: integer;
             Polynom: std_logic_vector
        );
        port (
             CLK, RST: in std_logic;
             Q: out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    signal CLK: std_logic := '0';
    signal RST: std_logic := '0';
    signal Q: std_logic_vector(N-1 downto 0);
begin
    uut: lfsr_int
        generic map(
            N => N,
            Polynom => Polynom
        )
        port map(
            CLK => CLK,
            RST => RST,
            Q => Q
        );
    
    clk_process: process
    begin
        CLK <= '0';
        wait for DELAY / 2;
        CLK <= '1';
        wait for DELAY / 2;
    end process;
    
    stim_proc: process
    begin
        RST <= '1';
        wait for DELAY*2;
        RST <= '0';

        wait for DELAY*35;

        RST <= '1';
        wait for DELAY*2;
        RST <= '0';
        
        wait for DELAY*10;
    end process;

end Behavioral;
