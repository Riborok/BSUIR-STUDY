import { useCallback, useEffect, useState } from "react";
import Quill from "quill";
import QuillCursors from "quill-cursors";
import "quill/dist/quill.snow.css";
import { io } from "socket.io-client";
import { useParams } from "react-router-dom";

let Font = Quill.import('formats/font');
Font.whitelist = ['arial', 'verdana', 'tahoma', 'times-new-roman', 'helvetica', 'sans-serif'];
Quill.register(Font, true);
Quill.register('modules/cursors', QuillCursors);

const SAVE_INTERVAL_MS = 2000;
const TOOLBAR_OPTIONS = [
    [{ header: [1, 2, 3, 4, 5, 6, false] }],
    [{ 'font': ['arial', 'verdana', 'tahoma', 'times-new-roman', 'helvetica', 'sans-serif'] }],
    [{ list: "ordered" }, { list: "bullet" }],
    ["bold", "italic", "underline"],
    [{ color: [] }, { background: [] }],
    [{ script: "sub" }, { script: "super" }],
    [{ align: [] }],
    ["image", "blockquote", "code-block"],
    ["clean"],
];

export default function TextEditor() {
    const { id: documentId } = useParams();
    const [socket, setSocket] = useState(null);
    const [quill, setQuill] = useState(null);
    const [cursorModule, setCursorModule] = useState(null);
    const [connectedUsers, setConnectedUsers] = useState([]);

    useEffect(() => {
        const s = io("http://localhost:3001");
        setSocket(s);

        return () => {
            if (s) s.disconnect();
        };
    }, []);

    useEffect(() => {
        if (!socket || !quill) return;

        const loadDocument = async () => {
            socket.once("load-document", document => {
                quill.setContents(document);
                quill.enable();
            });

            socket.emit("get-document", documentId);
        };

        loadDocument();
    }, [documentId, socket, quill]);

    useEffect(() => {
        if (!socket || !quill) return;

        const interval = setInterval(() => {
            socket.emit("save-document", quill.getContents());
        }, SAVE_INTERVAL_MS);

        return () => {
            clearInterval(interval);
        };
    }, [socket, quill]);

    useEffect(() => {
        if (!socket || !quill) return;

        const handleReceiveChanges = delta => {
            quill.updateContents(delta);
        };

        socket.on("receive-changes", handleReceiveChanges);

        return () => {
            socket.off("receive-changes", handleReceiveChanges);
        };
    }, [socket, quill]);

    useEffect(() => {
        if (!socket || !quill) return;

        const handleTextChange = (delta, oldDelta, source) => {
            if (source !== "user") return
            socket.emit("send-changes", delta)
        }
        quill.on("text-change", handleTextChange)

        return () => {
            quill.off("text-change", handleTextChange)
        }
    }, [socket, quill]);

    useEffect(() => {
        if (!socket || !quill || !cursorModule) return;

        const handleCursorPosition = range => {
            if (range) {
                socket.emit("send-cursor-position", range);
            }
        };

        quill.on("selection-change", handleCursorPosition);

        return () => {
            quill.off("selection-change", handleCursorPosition);
        };
    }, [socket, quill, cursorModule]);

    useEffect(() => {
        if (!socket || !cursorModule) return;

        const handleReceiveCursorPosition = ({ userId, userName, userColor, position }) => {
            cursorModule.createCursor(userId, userName, userColor);
            cursorModule.moveCursor(userId, position);
        };

        socket.on("receive-cursor-position", handleReceiveCursorPosition);

        const handleLoadCursors = cursors => {
            Object.keys(cursors).forEach(userId => {
                const { userName, userColor, position } = cursors[userId];
                cursorModule.createCursor(userId, userName, userColor);
                cursorModule.moveCursor(userId, position);
            });
        };

        socket.on("load-cursors", handleLoadCursors);

        const handleRemoveCursor = userId => {
            cursorModule.removeCursor(userId);
        };

        socket.on("remove-cursor", handleRemoveCursor);

        return () => {
            socket.off("receive-cursor-position", handleReceiveCursorPosition);
            socket.off("load-cursors", handleLoadCursors);
            socket.off("remove-cursor", handleRemoveCursor);
        };
    }, [socket, cursorModule]);

    useEffect(() => {
        if (!socket) return;

        socket.on("user-connected", user => {
            setConnectedUsers(prevUsers => [...prevUsers, user]);
        });

        socket.on("user-disconnected", userId => {
            setConnectedUsers(prevUsers => prevUsers.filter(user => user.userId !== userId));
        });

        socket.on("load-users", users => {
            setConnectedUsers(users);
        });

        return () => {
            socket.off("user-connected");
            socket.off("user-disconnected");
            socket.off("load-users");
        };
    }, [socket]);

    const wrapperRef = useCallback(wrapper => {
        if (wrapper == null) return;

        wrapper.innerHTML = "";
        const editor = document.createElement("div");
        wrapper.append(editor);
        const q = new Quill(editor, {
            theme: "snow",
            modules: {
                toolbar: TOOLBAR_OPTIONS,
                cursors: true
            },
        });
        q.disable();
        q.setText("Loading...");
        setQuill(q);
        setCursorModule(q.getModule('cursors'));
    }, []);

    return (
        <div>
            <div className="connected-users">
                {connectedUsers.map(user => (
                    <div key={user.userId} className="user" style={{backgroundColor: user.userColor}}>
                        <div className="user-initial">{user.userName}</div>
                    </div>
                ))}
            </div>
            <div className="container" ref={wrapperRef}></div>
        </div>
    );
}
