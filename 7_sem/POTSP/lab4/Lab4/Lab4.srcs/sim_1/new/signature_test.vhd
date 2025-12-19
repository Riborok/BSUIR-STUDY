library IEEE;
use IEEE.STD_LOGIC_1164.ALL;

-- Uncomment the following library declaration if using
-- arithmetic functions with Signed or Unsigned values
--use IEEE.NUMERIC_STD.ALL;

-- Uncomment the following library declaration if instantiating
-- any Xilinx leaf cells in this code.
--library UNISIM;
--use UNISIM.VComponents.all;

entity signature_test is
end signature_test;

architecture Behavioral of signature_test is
    constant N: integer := 3;
    constant Polynom: std_logic_vector(N-1 downto 0) := "011";
    constant DELAY: time := 10 ns;
    
    component signature_analyzer is
        generic (
             N: integer;
             Polynom: std_logic_vector
        );
        port (
             CLK, RST, Din: in std_logic;
             Sign: out std_logic_vector(N-1 downto 0)
        );
    end component;
    
    signal CLK: std_logic := '0';
    signal RST: std_logic;
    signal Din: std_logic;
    signal Sign: std_logic_vector(N-1 downto 0);
begin
    uut: signature_analyzer
        generic map(
            N => N,
            Polynom => Polynom
        )
        port map(
            CLK => CLK,
            RST => RST,
            Din => Din,
            Sign => Sign
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
        Din <= '0';
        wait for DELAY*2;
        RST <= '0';
        wait for DELAY;
        
        Din <= '1';
        wait for DELAY;
        
        Din <= '1';
        wait for DELAY;
        
        Din <= '0'; 
        wait for DELAY;
        
        Din <= '0';
        wait for DELAY;
        
        Din <= '0';
        wait for DELAY;
        
        Din <= '0';
        wait for DELAY;
        
        Din <= '1';
        wait for DELAY;
        
        Din <= '1';
        wait for DELAY;
                
        RST <= '1';
        Din <= '0';
        wait for DELAY;
        RST <= '0';
        wait for DELAY;
        
        Din <= '1';
        wait for DELAY;
        
        Din <= '0';
        wait for DELAY;
    end process;
end Behavioral;
