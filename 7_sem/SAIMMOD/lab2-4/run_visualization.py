import os
import http.server
import socketserver
import webbrowser
from pathlib import Path
import time
import threading
import subprocess
import sys
import hashlib

# Path to simulation script
simulation_script = Path(__file__).parent / 'gas_station_simulation.py'
html_file_path = Path(__file__).parent / 'gas_station_visualization.html'

# Start a simple HTTP server
PORT = 8000

# Cache for simulation script hash
simulation_cache = {
    'hash': None,
    'last_run': None
}

def get_file_hash(file_path):
    """Calculate MD5 hash of a file"""
    hash_md5 = hashlib.md5()
    try:
        with open(file_path, 'rb') as f:
            for chunk in iter(lambda: f.read(4096), b""):
                hash_md5.update(chunk)
        return hash_md5.hexdigest()
    except FileNotFoundError:
        return None

def should_run_simulation():
    """Check if simulation script has changed since last run"""
    current_hash = get_file_hash(simulation_script)
    if current_hash is None:
        return True  # File not found, try to run anyway

    if simulation_cache['hash'] != current_hash:
        simulation_cache['hash'] = current_hash
        simulation_cache['last_run'] = time.time()
        return True

    print("✨ Using cached simulation result (script unchanged)")
    return False

class MyHTTPRequestHandler(http.server.SimpleHTTPRequestHandler):
    def do_GET(self):
        if self.path.startswith('/visualization.html'):
            # Check if we need to run simulation
            if should_run_simulation():
                print("🔄 Running simulation to update trace data...")
                try:
                    result = subprocess.run(
                        [sys.executable, str(simulation_script)],
                        capture_output=True,
                        text=True,
                        cwd=simulation_script.parent,
                        timeout=60
                    )
                    if result.returncode == 0:
                        print("✅ Simulation completed successfully")
                    else:
                        print(f"⚠️ Simulation returned code {result.returncode}")
                        print(f"Error: {result.stderr}")
                except subprocess.TimeoutExpired:
                    print("⚠️ Simulation timeout (>60s)")
                except Exception as e:
                    print(f"⚠️ Error running simulation: {e}")

            # Read updated trace data
            trace_file_path = Path(__file__).parent / 'azs_trace.txt'
            try:
                with open(trace_file_path, 'r', encoding='utf-8') as f:
                    trace_data = f.read()
            except FileNotFoundError:
                trace_data = "// No trace data available"
                print("⚠️ azs_trace.txt not found")

            # Read HTML template
            with open(html_file_path, 'r', encoding='utf-8') as f:
                html_content = f.read()

            self.send_response(200)
            self.send_header("Content-type", "text/html; charset=utf-8")
            self.send_header('Cache-Control', 'no-store, no-cache, must-revalidate, max-age=0')
            self.send_header('Pragma', 'no-cache')
            self.send_header('Expires', '0')
            self.end_headers()
            final_html_content = html_content.replace('/*__TRACE_DATA__*/', trace_data)
            self.wfile.write(final_html_content.encode('utf-8'))
        else:
            super().do_GET()

os.chdir(Path(__file__).parent)

# Add timestamp to URL to force browser reload
timestamp = int(time.time())
url = f'http://localhost:{PORT}/visualization.html?v={timestamp}'

print(f"\n🚀 Starting web server on port {PORT}...")
print(f"🌐 Open your browser at: {url}")
print("Press Ctrl+C to stop the server\n")

httpd = socketserver.TCPServer(("", PORT), MyHTTPRequestHandler)

# Run the server in a separate thread
server_thread = threading.Thread(target=httpd.serve_forever)
server_thread.daemon = True
server_thread.start()

# A small delay to allow the server to start before opening the browser
time.sleep(1)

# Open browser automatically
webbrowser.open(url)

# Keep the main thread alive to handle Ctrl+C
try:
    while True:
        time.sleep(1)
except KeyboardInterrupt:
    print("\n\n🛑 Stopping server...")
    httpd.shutdown()
    print("✅ Server stopped.")
