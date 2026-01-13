


// async function login() {
//   const email = document.getElementById("email").value;
//   const password = document.getElementById("password").value;

//   try {
//     const response = await fetch(`${CONFIG.API_BASE_URL}/auth/login`, {
//       method: "POST",
//       headers: { "Content-Type": "application/json" },
//       body: JSON.stringify({ email, password })
//     });

//     const result = await response.json();

//     if (!result.success) {
//       throw new Error(result.message);
//     }

//     localStorage.setItem("token", result.data.token);
//     window.location.href = "chat.html";
//   } catch (err) {
//     document.getElementById("error").innerText = err.message;
//   }
// }


// Form switching functions
function switchToSignup() {
  document.getElementById('loginForm').classList.remove('active');
  document.getElementById('signupForm').classList.add('active');
}

function switchToLogin() {
  document.getElementById('signupForm').classList.remove('active');
  document.getElementById('loginForm').classList.add('active');
}

// Login function (existing logic)
async function login() {
  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;
  const errorElement = document.getElementById("error");

  try {
    // Basic validation
    if (!email || !password) {
      throw new Error("Please enter both email and password.");
    }

    const response = await fetch(`${CONFIG.API_BASE_URL}/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password })
    });

    const result = await response.json();

    if (!result.success) {
      throw new Error(result.message);
    }

    localStorage.setItem("token", result.data.token);
    window.location.href = "chat.html";
  } catch (err) {
    errorElement.innerText = err.message;
    errorElement.classList.add('show');
  }
}

// Signup function (new logic only)
async function signup() {
  const fullName = document.getElementById('signupName').value;
  const email = document.getElementById('signupEmail').value;
  const password = document.getElementById('signupPassword').value;
  const confirmPassword = document.getElementById('confirmPassword').value;
  const errorElement = document.getElementById('error');

  try {
    if (!fullName || !email || !password || !confirmPassword) {
      throw new Error("Please fill in all fields.");
    }

    if (password !== confirmPassword) {
      throw new Error("Passwords do not match.");
    }

    const response = await fetch(`${CONFIG.API_BASE_URL}/auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        fullName: fullName,   // ✅ MUST match DTO
        email: email,
        password: password
      })
    });

    const result = await response.json();

    if (!response.ok) {
      throw new Error(result.message || "Registration failed");
    }

    // Your backend DOES NOT return token on register
    // So we redirect to login
    switchToLogin();
    errorElement.innerText = "Account created successfully. Please login.";
    errorElement.classList.add("show");

  } catch (err) {
    errorElement.innerText = err.message;
    errorElement.classList.add("show");
  }
}


// Placeholder functions for other buttons (no logic added as requested)
function signInWithGoogle() {
  document.getElementById('error').innerText = "Google sign in not implemented yet.";
  document.getElementById('error').classList.add('show');
}

function forgotPassword() {
  document.getElementById('error').innerText = "Password reset not implemented yet.";
  document.getElementById('error').classList.add('show');
}
let googleInitialized = false;

function googleLogin() {
  if (!googleInitialized) {
    google.accounts.id.initialize({
      client_id: "426980312320-lu2v69fj0o6eod92d5j5ureeqm052k0q.apps.googleusercontent.com",
      callback: handleGoogleResponse,
      ux_mode: "popup"
    });

    googleInitialized = true;
  }

  google.accounts.id.prompt();
}

async function handleGoogleResponse(response) {
  const errorElement = document.getElementById("error");

  try {
    const res = await fetch(`${CONFIG.API_BASE_URL}/auth/google-login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        idToken: response.credential
      })
    });

    const result = await res.json();

    if (!result.success) {
      throw new Error(result.message);
    }

    localStorage.setItem("token", result.data.token);
    window.location.href = "chat.html";
  } catch (err) {
    errorElement.innerText = err.message;
    errorElement.classList.add("show");
  }
}
