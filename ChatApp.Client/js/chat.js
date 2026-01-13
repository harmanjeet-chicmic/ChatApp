// /* ---------------- AUTH CHECK ---------------- */
// /* ---------------- AUTH TOKEN (SAFE) ---------------- */
// if (!window.__chatToken) {
//   window.__chatToken = localStorage.getItem("token");
// }

// const token = window.__chatToken;

// if (!token) {
//   window.location.href = "index.html";
// }

// /* ---------------- CONFIG ---------------- */
// const headers = {
//   Authorization: `Bearer ${token}`,
//   "Content-Type": "application/json",
// };

// const chatList = document.querySelector(".chat-list");
// const chatMessages = document.querySelector(".chat-messages");
// const chatHeaderName = document.querySelector(".chat-header .chat-name");
// const chatHeaderStatus = document.querySelector(".chat-header .chat-status");
// const messageInput = document.querySelector(".chat-input input");

// /* ---------------- STATE ---------------- */
// let activeConversationId = null;
// let selectedUser = null;
// let connection = null;

// /* ---------------- CURRENT USER ---------------- */
// function getCurrentUser() {
//   const payload = JSON.parse(atob(token.split(".")[1]));
//   return {
//     userId: payload.nameidentifier,
//     fullName: payload.fullName || payload["fullName"],
//   };
// }

// const currentUser = getCurrentUser();

// /* ---------------- SIGNALR ---------------- */
// async function startSignalR() {
//   if (typeof signalR === "undefined") {
//     console.error("SignalR library not loaded");
//     return;
//   }

//   connection = new signalR.HubConnectionBuilder()
//     .withUrl(CONFIG.SIGNALR_URL, {
//       accessTokenFactory: () => token,
//     })
//     .withAutomaticReconnect()
//     .build();

//   connection.on("ReceiveMessage", (message) => {
//     if (message.conversationId !== activeConversationId) return;
//     appendMessage(message.senderId, message.content, message.sentAt);
//   });

//   connection.on("UserOnline", (userId) => updateUserStatus(userId, true));
//   connection.on("UserOffline", (userId) => updateUserStatus(userId, false));

//   try {
//     await connection.start();
//     console.log("SignalR connected");
//   } catch (err) {
//     console.error("SignalR connection failed", err);
//   }
// }

// function appendMessage(senderId, content, sentAt) {
//   const isMine = senderId === currentUser.userId;

//   const div = document.createElement("div");
//   div.className = "message " + (isMine ? "sent" : "received");

//   div.innerHTML = `
//     <p>${content}</p>
//     <span class="msg-time">
//       ${new Date(sentAt).toLocaleTimeString([], {
//         hour: "2-digit",
//         minute: "2-digit",
//       })}
//     </span>
//   `;

//   chatMessages.appendChild(div);
//   chatMessages.scrollTop = chatMessages.scrollHeight;
// }

// function updateUserStatus(userId, isOnline) {
//   document.querySelectorAll(".chat-item").forEach((item) => {
//     if (item.dataset.userId === userId) {
//       item.querySelector(".chat-last-msg").innerText = isOnline
//         ? "Online"
//         : "Offline";
//     }
//   });
// }

// /* ---------------- LOAD USERS ---------------- */
// async function loadUsers() {
//   const res = await fetch(`${CONFIG.API_BASE_URL}/users`, { headers });
//   const result = await res.json();

//   chatList.innerHTML = "";

//   result.data.forEach((user) => {
//     if (user.userId === currentUser.userId) return;

//     const div = document.createElement("div");
//     div.className = "chat-item";
//     div.dataset.userId = user.userId;

//     div.innerHTML = `
//       <div class="avatar">${user.fullName.charAt(0)}</div>
//       <div class="chat-info">
//         <div class="chat-name">${user.fullName}</div>
//         <div class="chat-last-msg">
//           ${user.isOnline ? "Online" : "Offline"}
//         </div>
//       </div>
//     `;

//     div.onclick = () => openChat(user);
//     chatList.appendChild(div);
//   });
// }

// /* ---------------- OPEN CHAT ---------------- */
// async function openChat(user) {
//   selectedUser = user;
//   chatHeaderName.innerText = user.fullName;
//   chatHeaderStatus.innerText = user.isOnline ? "online" : "offline";
//   chatMessages.innerHTML = "";

//   const res = await fetch(
//     `${CONFIG.API_BASE_URL}/chat/conversation?receiverId=${user.userId}`,
//     { method: "POST", headers }
//   );

//   const result = await res.json();
//   activeConversationId = result.data.conversationId;
//   if (!connection || connection.state !== "Connected") {
//     console.warn("SignalR not connected yet");
//     return;
//   }

//   await connection.invoke("JoinConversation", activeConversationId);

//   loadMessages();
// }

// /* ---------------- LOAD MESSAGES ---------------- */
// async function loadMessages() {
//   if (!activeConversationId) return;

//   const res = await fetch(
//     `${CONFIG.API_BASE_URL}/chat/messages/${activeConversationId}`,
//     { headers }
//   );

//   if (!res.ok) return;

//   const result = await res.json();
//   chatMessages.innerHTML = "";

//   result.data.forEach((m) => {
//     appendMessage(m.senderId, m.content, m.createdAt);
//   });
// }

// /* ---------------- SEND MESSAGE ---------------- */
// async function sendMessage() {
//   const content = messageInput.value.trim();
//   if (!content || !activeConversationId) return;

//   await connection.invoke("SendMessage", activeConversationId, content);
//   messageInput.value = "";
// }

// /* ---------------- ENTER KEY ---------------- */
// messageInput.addEventListener("keydown", (e) => {
//   if (e.key === "Enter") sendMessage();
// });

// /* ---------------- INIT ---------------- */
// startSignalR();
// loadUsers();
// // ---------------------------------------------
// // Force SignalR disconnect when tab is closed
// // ---------------------------------------------
// window.addEventListener("beforeunload", () => {
//   if (connection) {
//     connection.stop();
//   }
// });

//---------------------------------------------------------------------------------------
// const token = localStorage.getItem("token");
// if (!token) window.location.href = "index.html";
// let joinedConversationId = null;

// /* ================= CONFIG ================= */
// const headers = {
//   Authorization: `Bearer ${token}`,
//   "Content-Type": "application/json",
// };

// const chatList = document.querySelector(".chat-list");
// const chatMessages = document.querySelector(".chat-messages");
// const chatHeaderName = document.querySelector(".chat-header .chat-name");
// const chatHeaderStatus = document.querySelector(".chat-header .chat-status");
// const messageInput = document.querySelector(".chat-input input");

// /* ================= STATE ================= */
// let activeConversation = null;
// let connection = null;
// const onlineUsers = new Set();

// // pagination
// let oldestMessageTime = null;
// let isLoading = false;
// let hasMore = true;

// /* ================= CURRENT USER ================= */
// function getCurrentUser() {
//   const payload = JSON.parse(atob(token.split(".")[1]));
//   return {
//     userId: payload.sub,
//     fullName: payload.fullName,
//   };
// }
// const currentUser = getCurrentUser();

// /* ================= SIGNALR ================= */
// async function startSignalR() {
//   connection = new signalR.HubConnectionBuilder()
//     .withUrl(CONFIG.SIGNALR_URL, {
//       accessTokenFactory: () => token,
//     })
//     .withAutomaticReconnect()
//     .build();

//   connection.on("ReceiveMessage", handleIncomingMessage);

//   connection.on("UserOnline", (userId) => {
//     onlineUsers.add(userId);
//     updatePresenceUI(userId, true);
//   });

//   connection.on("UserOffline", (userId) => {
//     onlineUsers.delete(userId);
//     updatePresenceUI(userId, false);
//   });

//   await connection.start();
// }

// /* ================= CONVERSATIONS ================= */
// async function loadConversations() {
//   const res = await fetch(`${CONFIG.API_BASE_URL}/chat/conversations`, {
//     headers,
//   });
//   const result = await res.json();
//   chatList.innerHTML = "";
//   result.data.forEach(renderConversation);
// }

// function renderConversation(c) {
//   const isGroup = c.isGroup;
//   const isOnline = !isGroup && onlineUsers.has(c.otherUserId);

//   const title = isGroup ? c.groupName : c.otherUserName;
//   const avatarText = title.charAt(0).toUpperCase();

//   const div = document.createElement("div");
//   div.className = "chat-item";
//   div.dataset.conversationId = c.conversationId;

//   if (!isGroup) {
//     div.dataset.userId = c.otherUserId;
//   }

//   div.innerHTML = `
//     <div class="avatar ${isOnline ? "online" : ""}">
//       ${avatarText}
//     </div>
//     <div class="chat-info">
//       <div class="chat-name">${title}</div>
//       <div class="chat-last-msg">
//         ${c.lastMessage || "Tap to start chatting"}
//       </div>
//     </div>
//     ${
//       c.unreadCount > 0
//         ? `<span class="badge">${c.unreadCount}</span>`
//         : ""
//     }
//   `;

//   div.onclick = () => openConversation(c);
//   chatList.appendChild(div);
// }

// /* ================= OPEN CHAT ================= */
// async function openConversation(c) {
//   activeConversation = c;

//   if (joinedConversationId !== c.conversationId) {
//     await connection.invoke("JoinConversation", c.conversationId);
//     joinedConversationId = c.conversationId;
//   }

//   chatHeaderName.innerText = c.isGroup ? c.groupName : c.otherUserName;
//   chatHeaderStatus.innerText = c.isGroup
//     ? "Group"
//     : onlineUsers.has(c.otherUserId)
//     ? "online"
//     : "offline";

//   chatMessages.innerHTML = "";
//   oldestMessageTime = null;
//   hasMore = true;

//   await loadMessages();
//   await markAsRead(c.conversationId);
//   loadConversations();
// }

// /* ================= LOAD MESSAGES ================= */
// async function loadMessages() {
//   if (!activeConversation || isLoading || !hasMore) return;
//   isLoading = true;

//   const url = new URL(
//     `${CONFIG.API_BASE_URL}/chat/messages/${activeConversation.conversationId}`
//   );

//   if (oldestMessageTime) {
//     url.searchParams.set("before", oldestMessageTime);
//   }
//   url.searchParams.set("limit", "10");

//   const prevHeight = chatMessages.scrollHeight;

//   const res = await fetch(url, { headers });
//   const result = await res.json();
//   const messages = result.data;

//   if (messages.length === 0) {
//     hasMore = false;
//     isLoading = false;
//     return;
//   }

//   oldestMessageTime = messages[0].createdAt;

//  messages.forEach((m) =>
//   renderMessage(
//     m.senderId,
//     m.senderName,
//     m.content,
//     m.createdAt,
//     true
//   )
// );

//   chatMessages.scrollTop =
//     chatMessages.scrollHeight - prevHeight;

//   isLoading = false;
// }

// /* ================= MESSAGE RENDER ================= */
// function renderMessage(senderId, senderName, content, time, prepend = false) {
//   const isMine = senderId === currentUser.userId;
//   const div = document.createElement("div");
//   div.className = `message ${isMine ? "sent" : "received"}`;

//   const showSender = activeConversation.isGroup && !isMine;

//   div.innerHTML = `
//     ${showSender ? `<strong>${senderName}</strong>` : ""}
//     <p>${content}</p>
//     <span class="msg-time">
//       ${new Date(time).toLocaleTimeString([], {
//         hour: "2-digit",
//         minute: "2-digit",
//       })}
//     </span>
//   `;

//   prepend ? chatMessages.prepend(div) : chatMessages.appendChild(div);
// }

// /* ================= SEND MESSAGE ================= */
// async function sendMessage() {
//   const content = messageInput.value.trim();
//   if (!content || !activeConversation) return;

//   // ✅ 1. IMMEDIATELY render message locally
//   renderMessage(
//     currentUser.userId,
//     currentUser.fullName,
//     content,
//     new Date().toISOString()
//   );

//   // ✅ 2. Send to server (for others)
//   await connection.invoke(
//     "SendMessage",
//     activeConversation.conversationId,
//     content
//   );

//   messageInput.value = "";
// }

// messageInput.addEventListener("keydown", (e) => {
//   if (e.key === "Enter") sendMessage();
// });

// /* ================= INCOMING MESSAGE ================= */
// function handleIncomingMessage(message) {
//   if (
//     activeConversation &&
//     message.conversationId === activeConversation.conversationId
//   ) {
//     renderMessage(
//       message.senderId,
//       message.senderName,
//       message.content,
//       message.sentAt,
//       false // realtime = append
//     );

//     // ✅ FORCE SCROLL TO BOTTOM
//     chatMessages.scrollTop = chatMessages.scrollHeight;

//     markAsRead(activeConversation.conversationId);
//   }

//   loadConversations();
// }

// /* ================= READ ================= */
// async function markAsRead(conversationId) {
//   await fetch(`${CONFIG.API_BASE_URL}/chat/read/${conversationId}`, {
//     method: "POST",
//     headers,
//   });
// }

// /* ================= PRESENCE ================= */
// function updatePresenceUI(userId, isOnline) {
//   document.querySelectorAll(".chat-item").forEach((item) => {
//     if (item.dataset.userId === userId) {
//       item.querySelector(".avatar").classList.toggle("online", isOnline);
//     }
//   });

//   if (
//     activeConversation &&
//     !activeConversation.isGroup &&
//     activeConversation.otherUserId === userId
//   ) {
//     chatHeaderStatus.innerText = isOnline ? "online" : "offline";
//   }
// }

// /* ================= SCROLL ================= */
// chatMessages.addEventListener("scroll", () => {
//   if (chatMessages.scrollTop === 0) {
//     loadMessages();
//   }
// });

// /* ================= CREATE GROUP ================= */
// document.addEventListener("DOMContentLoaded", () => {
//   const createGroupBtn = document.getElementById("createGroupBtn");
//   const groupModal = document.getElementById("groupModal");
//   const groupUsersList = document.getElementById("groupUsersList");
//   const groupNameInput = document.getElementById("groupNameInput");

//   createGroupBtn.onclick = openGroupModal;
//   document.getElementById("createGroupCancel").onclick = closeGroupModal;
//   document.getElementById("createGroupConfirm").onclick = createGroup;

//   async function openGroupModal() {
//     groupModal.classList.remove("hidden");
//     groupUsersList.innerHTML = "";

//     const res = await fetch(`${CONFIG.API_BASE_URL}/users`, { headers });
//     const result = await res.json();

//     result.data.forEach((u) => {
//       if (u.userId === currentUser.userId) return;

//       const div = document.createElement("div");
//       div.className = "user-item";
//       div.innerHTML = `
//         <input type="checkbox" value="${u.userId}" />
//         <span>${u.fullName}</span>
//       `;
//       groupUsersList.appendChild(div);
//     });
//   }

//   function closeGroupModal() {
//     groupModal.classList.add("hidden");
//     groupNameInput.value = "";
//   }

//   async function createGroup() {
//     const name = groupNameInput.value.trim();
//     if (!name) return alert("Group name required");

//     const memberIds = Array.from(
//       groupUsersList.querySelectorAll("input:checked")
//     ).map((cb) => cb.value);

//     if (memberIds.length === 0) {
//       return alert("Select at least one member");
//     }

//     await fetch(`${CONFIG.API_BASE_URL}/chat/group`, {
//       method: "POST",
//       headers,
//       body: JSON.stringify({ name, memberIds }),
//     });

//     closeGroupModal();
//     loadConversations();
//   }
// });

// /* ================= INIT ================= */
// startSignalR();
// loadConversations();

// window.addEventListener("beforeunload", () => {
//   if (connection) connection.stop();
// });
const chatForm = document.getElementById("chatForm");
const onlineUsers = new Set();

const token = localStorage.getItem("token");
if (!token) window.location.href = "index.html";

const headers = {
  Authorization: `Bearer ${token}`,
  "Content-Type": "application/json",
};

const chatList = document.getElementById("chatList");
const messages = document.getElementById("messages");
const chatTitle = document.getElementById("chatTitle");
const chatStatus = document.getElementById("chatStatus");
const messageInput = document.getElementById("messageInput");
const sendBtn = document.getElementById("sendBtn");

const userModal = document.getElementById("userModal");
const userList = document.getElementById("userList");
const groupModal = document.getElementById("groupModal");
const groupUsers = document.getElementById("groupUsers");
const groupName = document.getElementById("groupName");

let activeConversation = null;
let connection = null;

const payload = JSON.parse(atob(token.split(".")[1]));
const currentUser = { userId: payload.sub, fullName: payload.fullName };

console.log("👤 Logged in:", currentUser.fullName);

/* ================= SIGNALR ================= */
async function startSignalR() {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(CONFIG.SIGNALR_URL, { accessTokenFactory: () => token })
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveMessage", (m) => {
    console.log("📩 Message received:", m);

    // 🔥 Ignore my own messages (already rendered optimistically)
    if (m.senderId === currentUser.userId) return;

    if (
      activeConversation &&
      m.conversationId === activeConversation.conversationId
    ) {
      renderMessage(m.senderId, m.senderName, m.content, m.sentAt);
    }

    loadConversations();
  });

  connection.on("UserOnline", (id) => {
    console.log("🟢 User online:", id);
    onlineUsers.add(id);
    loadConversations(); // refresh sidebar
  });

  connection.on("UserOffline", (id) => {
    console.log("🔴 User offline:", id);
    onlineUsers.delete(id);
    loadConversations(); // refresh sidebar
  });

  await connection.start();
  console.log("✅ SignalR connected");
}

/* ================= CONVERSATIONS ================= */
async function loadConversations() {
  const res = await fetch(`${CONFIG.API_BASE_URL}/chat/conversations`, {
    headers,
  });
  const result = await res.json();

  chatList.innerHTML = "";

  if (!result.data || result.data.length === 0) {
    document.getElementById("emptyChats").classList.remove("hidden");
    return;
  }

  document.getElementById("emptyChats").classList.add("hidden");

  result.data.forEach((c) => {
    const isGroup = c.isGroup;
    const isOnline = !isGroup && onlineUsers.has(c.otherUserId);

    const title = isGroup ? c.groupName : c.otherUserName;
    const avatarText = title.charAt(0).toUpperCase();

    const div = document.createElement("div");
    div.className = "chat-item";

    div.innerHTML = `
      <div class="avatar ${isOnline ? "online" : ""}">
        ${avatarText}
      </div>
      <div class="chat-info">
        <div class="chat-name">${title}</div>
        <div class="chat-last-msg">${c.lastMessage || "Tap to start chatting"}</div>
      </div>
      ${
        c.unreadCount > 0
          ? `<span class="badge">${c.unreadCount}</span>`
          : ""
      }
    `;

    div.onclick = () => openConversation(c);
    chatList.appendChild(div);
  });
}


/* ================= OPEN CHAT ================= */
async function openConversation(c) {
  activeConversation = c;
  chatTitle.innerText = c.isGroup ? c.groupName : c.otherUserName;
  messages.innerHTML = "";

  await connection.invoke("JoinConversation", c.conversationId);
  console.log("📡 Joined conversation:", c.conversationId);

  const res = await fetch(
    `${CONFIG.API_BASE_URL}/chat/messages/${c.conversationId}`,
    { headers }
  );
  const result = await res.json();
  result.data.forEach((m) =>
    renderMessage(m.senderId, m.senderName, m.content, m.createdAt)
  );
}

/* ================= MESSAGE ================= */
function renderMessage(senderId, name, content, time) {
  const div = document.createElement("div");
  div.className =
    "message " + (senderId === currentUser.userId ? "sent" : "received");
  div.innerHTML = `<p>${content}</p><span class="msg-time">${new Date(
    time
  ).toLocaleTimeString()}</span>`;
  messages.appendChild(div);
  messages.scrollTop = messages.scrollHeight;
}

chatForm.onsubmit = (e) => {
  e.preventDefault();
  sendMessage();
};

async function sendMessage() {
  if (!messageInput.value || !activeConversation) return;
  console.log("📤 Trying to send:", messageInput.value, activeConversation);

  renderMessage(
    currentUser.userId,
    currentUser.fullName,
    messageInput.value,
    new Date()
  );
  await connection.invoke(
    "SendMessage",
    activeConversation.conversationId,
    messageInput.value
  );
  messageInput.value = "";
}

/* ================= NEW CHAT ================= */
document.getElementById("newChatBtn").onclick = openUserModal;
document.getElementById("startChatBtn").onclick = openUserModal;

async function openUserModal() {
  userModal.classList.remove("hidden");
  const res = await fetch(`${CONFIG.API_BASE_URL}/users`, { headers });
  const result = await res.json();

  userList.innerHTML = "";
  result.data
    .sort((a, b) => a.fullName.localeCompare(b.fullName))
    .forEach((u) => {
      if (u.userId === currentUser.userId) return;
      const div = document.createElement("div");
      div.className = "user-row";
      div.innerHTML = `<div class="user-avatar">${u.fullName[0]}</div><span>${u.fullName}</span>`;
      div.onclick = async () => {
        await fetch(
          `${CONFIG.API_BASE_URL}/chat/conversation?receiverId=${u.userId}`,
          { method: "POST", headers }
        );
        userModal.classList.add("hidden");
        loadConversations();
      };
      userList.appendChild(div);
    });
}

function closeUserModal() {
  userModal.classList.add("hidden");
}

/* ================= GROUP ================= */
document.getElementById("newGroupBtn").onclick = async () => {
  groupModal.classList.remove("hidden");
  const res = await fetch(`${CONFIG.API_BASE_URL}/users`, { headers });
  const result = await res.json();

  groupUsers.innerHTML = "";
  result.data.forEach((u) => {
    if (u.userId === currentUser.userId) return;
    groupUsers.innerHTML += `<div class="group-row"><input type="checkbox" value="${u.userId}" /> ${u.fullName}</div>`;
  });
};

document.getElementById("createGroupConfirm").onclick = async () => {
  const ids = Array.from(groupUsers.querySelectorAll("input:checked")).map(
    (i) => i.value
  );
  await fetch(`${CONFIG.API_BASE_URL}/chat/group`, {
    method: "POST",
    headers,
    body: JSON.stringify({ name: groupName.value, memberIds: ids }),
  });
  groupModal.classList.add("hidden");
  loadConversations();
};

function closeGroupModal() {
  groupModal.classList.add("hidden");
}
async function loadOnlineUsers() {
  const res = await fetch(`${CONFIG.API_BASE_URL}/users`, { headers });
  const result = await res.json();

  onlineUsers.clear();

  if (!result.data) return;

  result.data.forEach(u => {
    if (u.isOnline) {
      onlineUsers.add(u.userId);
    }
  });

  console.log("🟢 Currently online users:", Array.from(onlineUsers));
}


/* ================= INIT ================= */
(async () => {
  await startSignalR();
  await loadOnlineUsers();
  await loadConversations();
})();

setInterval(() => {
  if (connection && connection.state === "Connected") {
    connection.invoke("Heartbeat");
  }
}, 5000);
