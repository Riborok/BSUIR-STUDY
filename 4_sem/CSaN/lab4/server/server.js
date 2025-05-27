const mongoose = require("mongoose");
const Document = require("./Document");

mongoose.connect("mongodb://localhost/google-docs", {
  useNewUrlParser: true,
  useUnifiedTopology: true,
});

const { uniqueNamesGenerator, adjectives, animals } = require('unique-names-generator');
const randomColor = require('randomcolor');
const io = require("socket.io")(3001, {
  cors: {
    origin: "http://localhost:3000",
    methods: ["GET", "POST"],
  },
});

const users = {};

io.on("connection", socket => {
  const userName = uniqueNamesGenerator({
    dictionaries: [adjectives, animals],
    separator: '-',
  });
  const userColor = randomColor();

  socket.on("get-document", async documentId => {
    if (!users[documentId])
      users[documentId] = {};

    const userArray = Object.entries(users[documentId]).map(([userId, { userName, userColor }]) => ({
      userId,
      userName,
      userColor
    }));
    socket.emit("load-users", userArray);
    socket.broadcast.to(documentId).emit("user-connected", { userId: socket.id, userName, userColor });
    socket.join(documentId);

    users[documentId][socket.id] = { userName, userColor, position: null };
    const document = await findOrCreateDocument(documentId);
    socket.emit("load-document", document.data);
    socket.emit("load-cursors", users[documentId]);

    socket.on("send-changes", delta => {
      socket.broadcast.to(documentId).emit("receive-changes", delta)
    });

    socket.on("save-document", async data => {
      await Document.findByIdAndUpdate(documentId, { data })
    });

    socket.on("send-cursor-position", position => {
      users[documentId][socket.id] = { userName, userColor, position };
      socket.broadcast.to(documentId).emit("receive-cursor-position", { userId: socket.id, userName, userColor, position })
    });

    socket.on("disconnect", () => {
      delete users[documentId][socket.id];
      if (Object.keys(users[documentId]).length === 0)
        delete users[documentId];
      socket.broadcast.to(documentId).emit("remove-cursor", socket.id);
      socket.broadcast.to(documentId).emit("user-disconnected", socket.id);
    });
  });
});

async function findOrCreateDocument(id) {
  if (id == null) return

  const document = await Document.findById(id);
  if (document) return document
  return await Document.create({ _id: id, data: "" })
}
