import './css/App.css';
import './css/Font.css';
import './css/User.css';
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate
} from "react-router-dom"
import { v4 } from "uuid"
import TextEditor from "./pages/TextEditor";

function App() {
  return (
      <Router>
        <Routes>
          <Route path="/" element={<Navigate to={`/documents/${v4()}`} replace />} />
          <Route path="/documents/:id" element={<TextEditor />} />
        </Routes>
      </Router>
  )
}

export default App
