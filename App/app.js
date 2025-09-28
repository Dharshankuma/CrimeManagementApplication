/* --------------------------
       App state (dummy data)
       -------------------------- */
const state = {
  reports: [],
  investigations: [],
  evidence: [],
  criminals: [],
  users: [],
  logs: [],
  notifications: [],
};

function genId(prefix = "ID") {
  return prefix + "-" + Math.random().toString(36).slice(2, 9).toUpperCase();
}

function seed() {
  state.reports = [
    {
      id: "RPT-1001",
      title: "Vehicle break-in",
      type: "theft",
      location: "MG Road",
      reported: "2025-08-01",
      status: "Open",
    },
    {
      id: "RPT-1002",
      title: "Shop vandalism",
      type: "vandalism",
      location: "Market St",
      reported: "2025-08-02",
      status: "Under Investigation",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
    {
      id: "RPT-1003",
      title: "Online fraud",
      type: "fraud",
      location: "Cyber Hub",
      reported: "2025-08-04",
      status: "Resolved",
    },
  ];
  state.investigations = [
    {
      id: "INV-2001",
      caseId: "RPT-1001",
      lead: "Officer A",
      stage: "Evidence",
    },
    {
      id: "INV-2002",
      caseId: "RPT-1002",
      lead: "Detective B",
      stage: "Interrogation",
    },
  ];
  state.evidence = [
    {
      id: "EVD-3001",
      type: "Photo",
      desc: "CCTV still",
      caseId: "RPT-1001",
      location: "Locker A",
    },
  ];
  state.criminals = [
    {
      id: "CR-4001",
      name: "John Doe",
      aliases: "JD",
      risk: "High",
      lastSeen: "2025-07-20",
    },
  ];
  state.users = [
    {
      uid: "U-1",
      name: "Inspector D",
      role: "Admin",
      station: "Central",
      lastLogin: "2025-08-06",
    },
  ];
  state.logs = [
    {
      time: "2025-08-06 09:21",
      user: "U-1",
      action: "Login",
      details: "Successful",
    },
  ];
  state.notifications = [
    { id: "N-1", message: "New report assigned RPT-1002", time: "3h" },
  ];
}
const APIUrl = "https://localhost:7192/api";
let userIdentifier = "";
let userId = 0;
let bearer = "";
/* --------------------------
       Render functions
       -------------------------- */
function renderKPIs() {
  $("#kpiReports").text(state.reports.length);
  $("#kpiInvestigations").text(state.investigations.length);
  $("#kpiEvidence").text(state.evidence.length);
  $("#kpiCriminals").text(state.criminals.length);
  $("#kpiHigh").text(
    state.reports.filter((r) => r.status === "Under Investigation").length
  );
  $("#kpiAvgDays").text(14);
  $("#kpiAlerts").text(state.criminals.length);
}

function renderRecentReports() {
  const $t = $("#tblRecentReports").empty();
  state.reports.slice(0, 10).forEach((r) => {
    $t.append(`<tr>
          <td>${r.id}</td>
          <td>${escape(r.title)}</td>
          <td>${escape(r.type)}</td>
          <td>${escape(r.location)}</td>
          <td>${r.reported}</td>
          <td><span class="status_badge ${
            r.status === "Resolved"
              ? "bg-success"
              : r.status === "Open"
              ? "bg-warning text-dark"
              : "bg-info"
          }">${r.status}</span></td>
        </tr>`);
  });
}

// $(document).on("click", "#reportsPagination .page-link", function (e) {
//   e.preventDefault();
//   const selectedPage = parseInt($(this).data("page"));
//   if (!isNaN(selectedPage)) {
//     renderTables(selectedPage);
//   }
// });

/* --------------------------
       Chart
       -------------------------- */
let trendsChart = null;
function renderChart() {
  const ctx = document.getElementById("chartTrends").getContext("2d");
  const labels = Array.from({ length: 14 }).map((_, i) => {
    const d = new Date();
    d.setDate(d.getDate() - (13 - i));
    return d.toLocaleDateString();
  });
  const data = labels.map(() => Math.floor(Math.random() * 8) + 1);
  if (trendsChart) {
    trendsChart.data.labels = labels;
    trendsChart.data.datasets[0].data = data;
    trendsChart.update();
    return;
  }
  trendsChart = new Chart(ctx, {
    type: "line",
    data: {
      labels,
      datasets: [
        {
          label: "Reports",
          data,
          fill: true,
          tension: 0.3,
          borderWidth: 2,
        },
      ],
    },
    options: {
      plugins: { legend: { display: false } },
      scales: {
        x: { grid: { display: false } },
        y: { beginAtZero: true, ticks: { stepSize: 5 } },
      },
    },
  });
}

/* --------------------------
       SPA nav & events
       -------------------------- */
$(function () {
  $("#loader").hide();
  seed();
  $("#crime_report_date").datepicker({
    dateFormat: "dd-mm-yy", // You can set your desired date format
    maxDate: 0, // Optionally, prevent selecting future dates
  });

  // $("#topbar").hide();
  initAuth();
});

function initAuth() {
  const storedToken = localStorage.getItem("authToken");
  const storedUser = JSON.parse(localStorage.getItem("userDetails"));

  if (storedToken && storedUser) {
    const now = new Date().getTime();
    if (storedUser.expiryTime && now < storedUser.expiryTime) {
      // Token is still valid
      bearer = storedToken;
      userIdentifier = storedUser.userIdentifier || "";

      // Update UI
      $("#username_id").text(storedUser.userName);
      $("#view-login").hide();
      $("#topbar").show();
      $("#mainLayout").show();

      renderAll();
      applyRolePermissions(storedUser.roleId);
      DoGetConfiguration();
      return;
    }
  }

  // Token expired or not present → show login
  localStorage.removeItem("authToken");
  localStorage.removeItem("userDetails");
  showUnauthenticatedView("#view-login");
}

function renderAll() {
  renderKPIs();
  //enderRecentReports();
  //renderTables();
  renderChart();
}

function showUnauthenticatedView(viewId) {
  $("#topbar").hide().removeClass("d-flex");
  $("#mainLayout").hide();
  $(".view").hide();
  $(viewId).show();
}

$(document).on("click", "#forget_pass_link", function (e) {
  e.preventDefault();
  showUnauthenticatedView("#view-forgot-password");
  $("#forgotEmail").val(""); // Clear previous input
  $("#newPassword").val("");
  $("#confirmNewPassword").val("");
  $("#forgotError").hide();
  $("#forgotSuccess").hide();
});

$(document).on("click", "#register_Link", function (e) {
  e.preventDefault();
  showUnauthenticatedView("#view-register");
  $("#registerUsername").val("");
  $("#registerEmail").val("");
  $("#registerPassword").val("");
  $("#confirmPassword").val("");
  $("#registerError").hide();
  $("#registerSuccess").hide();
});

$(document).on("click", "#showLoginFromRegister", function (e) {
  e.preventDefault();
  showUnauthenticatedView("#view-login");
  $("#loginPassword").val("");
  $("#loginUsername").val("");
  $("#loginError").hide();
});

$("#log_out_page").on("click", function () {
  $("#topbar").hide();
  $("#mainLayout").hide();
  $("#view-login").show();
});

$("#loginForm").on("submit", async function (e) {
  await loginUser(e);
});

async function loginUser(e) {
  e.preventDefault();
  $("#loader").show();
  try {
    let userName = $("#loginUsername").val().trim();
    let password = $("#loginPassword").val().trim();

    // Wrap $.ajax in a promise to use await
    let response = await $.ajax({
      url: `${APIUrl}/Login/LoginUser`,
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify({ userName, password }),
      dataType: "json",
    });

    console.log("Login successful:", response);

    var data = response.data;
    var user = null;
    if (data.userDetails != null) {
      user = data.userDetails;
    }

    if (response.responseCode == 200) {
      const expiryTime = new Date();
      expiryTime.setMinutes(expiryTime.getMinutes() + data.expiryTime); // expiryTime in minutes

      localStorage.setItem("authToken", data.token || "dummy-token");
      localStorage.setItem(
        "userDetails",
        JSON.stringify({
          userName: user.userName,
          roleId: user.roleId,
          expiryTime: expiryTime.getTime(), // store as timestamp
          userIdentifier: user.userIdentifier,
        })
      );

      userIdentifier = user.userIdentifier;
      bearer = data.token;

      if (user && user.userName) {
        $("#username_id").text(user.userName);
      }
      renderAll();

      $("#view-login").hide();
      $("#topbar").show();
      $("#mainLayout").show();
      $("#loginPassword").val("");
      $("#loginUsername").val("");
      showView("dashboard"); // Show the default dashboard view

      // Apply role-based tab visibility
      applyRolePermissions(user.roleId);
      DoGetConfiguration();
    }
  } catch (error) {
    console.error(error);
    $("#error_body_txt").html(error.responseText || "An error occurred.");
    $("#error_modal").modal("show");
  } finally {
    $("#loader").hide();
  }
}

function applyRolePermissions(roleId) {
  $(".crime_users, .crime_audit_logs, .crime_investigation").show();

  if (roleId === 2) {
    $(".crime_users").hide();
    $(".crime_audit_logs").hide();
  } else if (roleId === 3) {
    $(".crime_users").hide();
    $(".crime_audit_logs").hide();
    $(".crime_investigation").hide();
  }
}

async function DoGetConfiguration() {
  $("#loader").show();
  try {
    const response = await $.ajax({
      url: `${APIUrl}/Configuration/GetConfiguration`,
      method: "GET",
      headers: { Authorization: `Bearer ${bearer}` },
    });

    console.log(response);

    if (response.responseCode == 200) {
      const data = response.data.data;

      const countryMaster = data.countryMaster || [];
      const crimeTypes = data.crimeTypes || [];
      const jurisdiction = data.jurisdictionMaster || [];
      const stateMaster = data.stateMaster || [];
      const statusMaster = data.statusMaster || [];

      // Populate Crime Types dropdown
      let crimeTypeHtml = `<option value="" selected>Select</option>`;
      crimeTypes.forEach((ct) => {
        crimeTypeHtml += `<option value="${ct.identifier}">${ct.name}</option>`;
      });
      $("#crime_type").html(crimeTypeHtml);
      //$("#crime_filterType").html(crimeTypeHtml);

      // Populate Country dropdown
      // let countryHtml = `<option value="" selected>Select</option>`;
      // countryMaster.forEach((c) => {
      //   countryHtml += `<option value="${c.id}">${c.name}</option>`;
      // });
      // $("#countrySelect").html(countryHtml);

      // // Populate State dropdown
      // let stateHtml = `<option value="" selected>Select</option>`;
      // stateMaster.forEach((s) => {
      //   stateHtml += `<option value="${s.id}">${s.name}</option>`;
      // });
      // $("#stateSelect").html(stateHtml);

      // Populate Jurisdiction dropdown
      let jurisdictionHtml = `<option value="" selected>Select</option>`;
      jurisdiction.forEach((j) => {
        jurisdictionHtml += `<option value="${j.identifer}">${j.name}</option>`;
      });
      $("#crime_jurisdiction").html(jurisdictionHtml);

      // Populate Status dropdown
      // let statusHtml = `<option value="" selected>Select</option>`;
      // statusMaster.forEach((st) => {
      //   statusHtml += `<option value="${st.id}">${st.name}</option>`;
      // });
      // $("#statusSelect").html(statusHtml);
    }
  } catch (error) {
    console.error("Error fetching configuration:", error);
    $("#error_body_txt").html(
      error.responseText || "Failed to load configuration"
    );
    $("#error_modal").modal("show");
  } finally {
    $("#loader").hide();
  }
}

$("#registerForm").on("submit", function (e) {
  registerUserDetails(e);
});

async function registerUserDetails(e) {
  e.preventDefault();
  $("#loader").show();

  try {
    let username = $("#registerUsername").val().trim();
    let email = $("#registerEmail").val().trim();
    let password = $("#registerPassword").val().trim();
    let confirmPassword = $("#confirmPassword").val().trim();

    $("#registerError").hide();
    $("#registerSuccess").hide();

    // Validate empty fields
    if (!username || !email || !password || !confirmPassword) {
      $("#error_body_txt").html("All fields are required.");
      $("#error_modal").modal("show");
      return; // stop execution
    }

    // Validate password match
    if (password !== confirmPassword) {
      $("#error_body_txt").html("Passwords do not match.");
      $("#error_modal").modal("show");
      return; // stop execution
    }

    // Prepare data
    var registerData = {
      userName: username,
      password: password,
      emailId: email, // was incorrectly using emailId variable
      confirmPassword: confirmPassword,
    };

    // Make AJAX call
    let response = await $.ajax({
      url: `${APIUrl}/Login/RegisterUser`,
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(registerData),
      dataType: "json",
    });

    console.log(response);

    if (response.responseCode == 200) {
      $("#success_body_txt").html("Registered Successfully. Continue to login");
      $("#success_modal").modal("show");
    }
  } catch (error) {
    console.error(error);
    $("#error_body_txt").html(error.responseText || "An error occurred.");
    $("#error_modal").modal("show");
  } finally {
    $("#loader").hide();
  }
}

function showView(name) {
  $(".nav-link").removeClass("active");
  $(`.nav-link[data-view="${name}"]`).addClass("active");
  $(".view").hide();
  $(`#view-${name}`).show();
  // scroll top container
  document.querySelector(".content").scrollTop = 0;
}

// initial
//showView("dashboard");

let tabid = 0;
const reportsPerPage = 10; // items per page
let currentPage = 0; // current page
// sidebar clicks
$(".nav-link").on("click", function (e) {
  e.preventDefault();
  const v = $(this).data("view");
  if (v) {
    showView(v);

    if (v == "crime-reporting") {
      tabid = 1;
    }
    if (v == "investigations") {
      tabid = 2;
    }

    if (v == "users") {
      tabid = 3;
    }
    currentPage = 0;
    renderTabs(tabid, currentPage);
  }
});

function renderTabs(tabid, page = 0) {
  switch (tabid) {
    case 1:
      renderCrimeTables(page);
      break;
    case 2:
      renderInvestigationTables(page);
      break;
  }
}

$(document).on("click", "#reportsPagination .page-link", function (e) {
  e.preventDefault();
  const selectedPage = parseInt($(this).data("page"));
  if (!isNaN(selectedPage)) {
    currentPage = selectedPage;
    renderTabs(tabid, currentPage);
  }
});

async function renderCrimeTables(page = 0) {
  const $tbl = $("#tblReports").empty();
  let htmlheader = "";
  let htmlbody = "";

  try {
    $("#loader").show();

    // Prepare request DTO for API
    const requestDTO = {
      PageNumber: page,
      PageSize: reportsPerPage,
      userIdentifier: userIdentifier,
    };

    // Build header
    htmlheader = `
      <thead>
        <tr>
          <th>S.no</th>
          <th>Title</th>
          <th>Type</th>
          <th>Location</th>
          <th>Reported</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>
    `;

    // Call API
    const response = await $.ajax({
      url: `${APIUrl}/CrimeReport/GetCrimeReports`,
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(requestDTO),
      dataType: "json",
      headers: { Authorization: `Bearer ${bearer}` },
    });

    console.log("Crime Reports:", response);

    if (response && response.data) {
      var crimeData = response.data.data;
      var totalCount = response.data.totalCount;

      if (crimeData && crimeData.length > 0) {
        htmlbody = "<tbody>";

        crimeData.forEach((r, index) => {
          // Decide status badge
          let statusBadge = "";
          switch (r.crimeStatusStr) {
            case "Resolved":
              statusBadge = `<span class="badge bg-success">${r.crimeStatusStr}</span>`;
              break;
            case "Open":
              statusBadge = `<span class="badge bg-warning text-dark">${r.crimeStatusStr}</span>`;
              break;
            case "Under Investigation":
              statusBadge = `<span class="badge bg-info">${r.crimeStatusStr}</span>`;
              break;
            default:
              statusBadge = `<span class="badge bg-secondary">${r.crimeStatusStr}</span>`;
          }

          htmlbody += `
            <tr>
              <td>${page * reportsPerPage + index + 1}</td>
              <td>${r.complaintName || "-"}</td>
              <td>${r.crimeType || "-"}</td>
              <td>${r.jurisdictionName || "-"}</td>
              <td>${r.crimeReportDate || "-"}</td>
              <td>${statusBadge}</td>
              <td>
                <button class="btn btn-sm btn-outline-primary view-report" data-id="${
                  r.reportIdentifer
                }">
                  <i class="fa-solid fa-eye"></i>
                </button>
              </td>
            </tr>
          `;
        });

        htmlbody += "</tbody>";
      } else {
        const colCount = $(htmlheader).find("th").length || 1;
        htmlbody = `<tbody><tr><td colspan="${colCount}" class="text-center">No Data Available</td></tr></tbody>`;
      }

      // ✅ Pagination
      const $pagination = $("#reportsPagination").empty();
      const totalPages = Math.ceil(totalCount / reportsPerPage);

      $pagination.append(
        `<li class="page-item ${page === 0 ? "disabled" : ""}">
          <a class="page-link" href="#" data-page="${page - 1}">Previous</a>
        </li>`
      );

      for (let i = 0; i < totalPages; i++) {
        $pagination.append(
          `<li class="page-item ${i === page ? "active" : ""}">
            <a class="page-link" href="#" data-page="${i}">${i + 1}</a>
          </li>`
        );
      }

      $pagination.append(
        `<li class="page-item ${page === totalPages - 1 ? "disabled" : ""}">
          <a class="page-link" href="#" data-page="${page + 1}">Next</a>
        </li>`
      );
    }
  } catch (err) {
    console.error("Error fetching crime reports:", err);
    $("#error_body_txt").html(
      err.responseText || "Failed to load crime reports"
    );
    $("#error_modal").modal("show");
  } finally {
    $("#tblReports").html(htmlheader + htmlbody);
    $("#loader").hide();
  }
}

async function renderInvestigationTables(page = 0) {
  const $tbl = $("#tblInvestigations").empty();
  let htmlheader = "";
  let htmlbody = "";

  try {
    $("#loader").show();

    // Prepare request DTO for API
    const requestDTO = {
      PageNumber: page,
      PageSize: reportsPerPage,
      userIdentifier: userIdentifier,
    };

    // Build table header
    htmlheader = `
      <thead>
        <tr>
          <th>S.no</th>
          <th>Case Name</th>
          <th>Crime Type</th>
          <th>Location</th>
          <th>Last Updated</th>
          <th>Status</th>
          <th>Actions</th>
        </tr>
      </thead>
    `;

    // Call API
    const response = await $.ajax({
      url: `${APIUrl}/Investigation/InvestigationGridDetails`,
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(requestDTO),
      dataType: "json",
      headers: { Authorization: `Bearer ${bearer}` },
    });

    if (response && response.data) {
      const crimeData = response.data.data || [];
      const totalCount = response.data.totalCount || 0;

      if (crimeData.length > 0) {
        htmlbody = "<tbody>";

        crimeData.forEach((r, index) => {
          // Status badge
          let statusBadge = "";
          switch (r.crimeStatus) {
            case "Resolved":
              statusBadge = `<span class="badge bg-success">${r.crimeStatus}</span>`;
              break;
            case "Open":
              statusBadge = `<span class="badge bg-warning text-dark">${r.crimeStatus}</span>`;
              break;
            case "Under Investigation":
              statusBadge = `<span class="badge bg-info">${r.crimeStatus}</span>`;
              break;
            default:
              statusBadge = `<span class="badge bg-secondary">${
                r.crimeStatus || "-"
              }</span>`;
          }

          htmlbody += `
            <tr>
              <td>${page * reportsPerPage + index + 1}</td>
              <td>${r.complaintName || "-"}</td>
              <td>${r.crimeType || "-"}</td>
              <td>${r.location || "-"}</td>
              <td>${r.lastUpdatedDate || "-"}</td>
              <td>${statusBadge}</td>
              <td>
                <button class="btn btn-sm btn-outline-primary view_invest" data-id="${
                  r.investigationIdentifer
                }">
                  <i class="fa-solid fa-eye"></i>
                </button>
                <button class="btn btn-sm btn-outline-primary update_invest" data-id="${
                  r.investigationIdentifer
                }">
                  <i class="fa-solid fa-pencil"></i>
                </button>
              </td>
            </tr>
          `;
        });

        htmlbody += "</tbody>";
      } else {
        const colCount = $(htmlheader).find("th").length || 1;
        htmlbody = `<tbody><tr><td colspan="${colCount}" class="text-center">No Data Available</td></tr></tbody>`;
      }

      // Pagination
      const $pagination = $("#invest_Pagination").empty();
      const totalPages = Math.ceil(totalCount / reportsPerPage);

      $pagination.append(
        `<li class="page-item ${page === 0 ? "disabled" : ""}">
          <a class="page-link" href="#" data-page="${page - 1}">Previous</a>
        </li>`
      );

      for (let i = 0; i < totalPages; i++) {
        $pagination.append(
          `<li class="page-item ${i === page ? "active" : ""}">
            <a class="page-link" href="#" data-page="${i}">${i + 1}</a>
          </li>`
        );
      }

      $pagination.append(
        `<li class="page-item ${page === totalPages - 1 ? "disabled" : ""}">
          <a class="page-link" href="#" data-page="${page + 1}">Next</a>
        </li>`
      );
    }
  } catch (err) {
    console.error("Error fetching crime reports:", err);
    $("#error_body_txt").html(
      err.responseText || "Failed to load crime reports"
    );
    $("#error_modal").modal("show");
  } finally {
    $("#loader").hide();
    $("#tblInvestigations").html(htmlheader + htmlbody);
  }
}

$(document).on("click", ".view_invest", function () {
  const identifier = $(this).data("id");
  fetchAndDisplayInvestigationDetails(identifier);
});

$(document).on("click", ".update_invest", function () {
  const identifier = $(this).data("id");
  fetchAndPopulateInvestigationForUpdate(identifier);
});

let currentInvestigationIdentifier = null;

// Function to fetch and display investigation details for the view modal
async function fetchAndDisplayInvestigationDetails(identifier) {
  try {
    $("#loader").show();
    const response = await $.ajax({
      url: `${APIUrl}/Investigation/InvestigationDetails?identifier=${identifier}`,
      method: "GET",
      headers: { Authorization: `Bearer ${bearer}` },
      dataType: "json",
    });
    console.log(response);
    if (response && response.responseCode) {
      $("#modalViewInvestigation").modal("show");
      //$("#error_modal").modal("show");
      const data = response.data;
      $("#viewComplaintName").text(data.complaintName || "-");
      $("#viewVictimName").text(data.complaintRaiseName);
      $("#viewCrimeType").text(data.crimeType || "-");
      $("#viewLocation").text(data.location || "-");
      $("#viewStatus").text(data.statusName || "-");
      $("#viewStartDate").text(data.startDateString);
      $("#viewEndDate").text(data.endDateString);

      $("#viewDescription").text(
        data.complaintDescription || "No description provided."
      );
    }
  } catch (err) {
    console.error("Error fetching investigation details for view:", err);
    $("#error_body_txt").html(
      err.responseJSON?.responseMessage ||
        "Failed to load investigation details."
    );
    $("#error_modal").modal("show");
  } finally {
    $("#loader").hide();
  }
}

async function fetchAndPopulateInvestigationForUpdate(identifier) {
  currentInvestigationIdentifier = identifier; // Store the identifier for update
  try {
    $("#loader").show();
    const response = await $.ajax({
      url: `${APIUrl}/Investigation/InvestigationDetails?identifier=${identifier}`,
      method: "GET",
      headers: { Authorization: `Bearer ${bearer}` },
      dataType: "json",
    });

    if (response && response.data) {
      const data = response.data;

      // Populate readonly fields
      $("#updateInvestigationIdentifier").val(data.identifier);
      $("#updateComplaintName").val(data.complaintName || "-");
      $("#updateCrimeType").val(data.crimeType || "-");
      $("#updateIoOfficer").val(data.ioOfficerName || "-");

      // Populate editable fields
      $("#updatePriority").val(data.priority || "");
      $("#updateDescription").val(data.investigationDescription || "");
      $("#updateEndDate").val(
        data.endDate ? new Date(data.endDate).toISOString().split("T")[0] : ""
      ); // Format date for input type="date"

      // Populate status dropdown dynamically
      //await populateStatusDropdown(data.statusIdentifier);

      $("#modalUpdateInvestigation").modal("show");
    } else {
      throw new Error("No data received for this investigation.");
    }
  } catch (err) {
    console.error("Error fetching investigation details for update:", err);
    $("#error_body_txt").html(
      err.responseJSON?.responseMessage ||
        "Failed to load investigation details for update."
    );
    $("#error_modal").modal("show");
  } finally {
    $("#loader").hide();
  }
}

// Optional: handle pagination click
$(document).on("click", "#invest_Pagination .page-link", function (e) {
  e.preventDefault();
  const page = parseInt($(this).data("page"));
  if (!isNaN(page) && page >= 0) {
    renderInvestigationTables(page);
  }
});

// Escape helper
function escape(s) {
  return s == null
    ? ""
    : String(s).replace(
        /[&<>]/g,
        (c) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;" }[c])
      );
}

// toggle collapse (desktop)
$("#btnToggle").on("click", function () {
  $("#mainLayout").toggleClass("collapsed");
});

// quick actions
$("#quickReportBtn, #btnQuickReport, #openNewReport, #btnQuickReport").on(
  "click",
  function () {
    new bootstrap.Modal(document.getElementById("modalNewReport")).show();
  }
);

$("#quickEvidenceBtn, #btnQuickEvidence, #openEvidenceModal").on(
  "click",
  function () {
    new bootstrap.Modal(document.getElementById("modalEvidence")).show();
  }
);

// new report submit
$("#save_crime").on("click", function (e) {
  //alert("hi");
  DoSaveCrimeReport(e);
});

async function DoSaveCrimeReport(e) {
  e.preventDefault();
  $("#loader").show();

  try {
    var complaintname = $("#complain_name").val();
    var crimeType = $("#crime_type").val();
    var jurisdiction = $("#crime_jurisdiction").val();
    var description = $("#crime_description").val();
    var crimePhoneno = $("#crime_raiser_no").val();
    var crimeDate = $("#crime_report_date").val();

    var data = {
      ComplaintName: complaintname,
      jurisdictionIdentifier: jurisdiction,
      crimeTypeIdentifier: crimeType,
      PhoneNumber: crimePhoneno,
      userIdentifier: userIdentifier,
      CrimeDescription: description,
      dateReportString: crimeDate,
    };

    console.log(data);

    const response = await $.ajax({
      url: `${APIUrl}/CrimeReport/RaiseCrimeReport`,
      method: "POST",
      contentType: "application/json",
      data: JSON.stringify(data),
      dataType: "json",
      headers: { Authorization: `Bearer ${bearer}` },
    });

    console.log(response);

    if (response && response.responseCode == 200) {
      DoClearValues();
      $("#modalNewReport").modal("hide"); // Bootstrap

      $("#success_body_txt").html("Complaint Raised Successfully");
      $("#success_modal").modal("show");

      renderCrimeTables();
    }
  } catch (error) {
    console.log(error);
  } finally {
    $("#loader").hide();
  }
}

function DoClearValues() {
  $("#complain_name").val("");
  $("#crime_type").val("");
  $("#crime_jurisdiction").val("");
  $("#crime_description").val("");
  $("#crime_raiser_no").val("");
  $("#dateReportString").val("");
}

// evidence submit
$("#formEvidence").on("submit", function (e) {
  e.preventDefault();
  const fd = new FormData(this);
  const rec = {
    id: genId("EVD"),
    type: fd.get("etype"),
    desc: fd.get("eDesc"),
    caseId: fd.get("caseId"),
    location: "Locker X",
  };
  state.evidence.unshift(rec);
  renderAll();
  bootstrap.Modal.getInstance(document.getElementById("modalEvidence")).hide();
});

// view / assign actions
$(document).on("click", ".view-report", function () {
  const id = $(this).data("id");
  const r = state.reports.find((x) => x.id === id);
  if (r) {
    $("#confirmBody").html(
      `<h6>${escape(r.title)}</h6><p><strong>Case:</strong> ${
        r.id
      }<br><strong>Type:</strong> ${r.type}<br><strong>Location:</strong> ${
        r.location
      }</p>`
    );
    new bootstrap.Modal(document.getElementById("modalConfirm")).show();
  }
});
$(document).on("click", ".assign-report", function () {
  const id = $(this).data("id");
  $("#confirmBody").text(`Assign ${id} to yourself?`);
  $("#confirmOk")
    .off("click")
    .on("click", function () {
      const r = state.reports.find((x) => x.id === id);
      if (r) r.assigned = "You";
      renderAll();
      bootstrap.Modal.getInstance(
        document.getElementById("modalConfirm")
      ).hide();
    });
  new bootstrap.Modal(document.getElementById("modalConfirm")).show();
});

// mark notification read
$(document).on("click", ".mark-read", function () {
  const id = $(this).data("id");
  state.notifications = state.notifications.filter((n) => n.id !== id);
  renderTables();
});

// global search (client-side)
$("#globalSearch").on("input", function () {
  const q = $(this).val().trim().toLowerCase();
  if (!q) {
    renderAll();
    return;
  }
  // simple highlight/filter: filter reports table rows
  $("#tblReports tr").each(function () {
    $(this).toggle($(this).text().toLowerCase().indexOf(q) !== -1);
  });
  $("#tblCriminals tr").each(function () {
    $(this).toggle($(this).text().toLowerCase().indexOf(q) !== -1);
  });
});

// filter apply
$("#applyFilter").on("click", function (e) {
  e.preventDefault();
  const type = $("#filterType").val();
  const from = $("#filterFrom").val();
  const to = $("#filterTo").val();
  let list = state.reports.slice();
  if (type) list = list.filter((r) => r.type === type);
  if (from) list = list.filter((r) => r.reported >= from);
  if (to) list = list.filter((r) => r.reported <= to);
  const $tbl = $("#tblReports").empty();
  list.forEach((r) =>
    $tbl.append(
      `<tr><td>${r.id}</td><td>${escape(r.title)}</td><td>${escape(
        r.type
      )}</td><td>${escape(r.location)}</td><td>${
        r.reported
      }</td><td><span class="status_badge ${
        r.status === "Resolved"
          ? "bg-success"
          : r.status === "Open"
          ? "bg-warning text-dark"
          : "bg-info"
      }">${
        r.status
      }</span></td><td><button class="btn btn-sm btn-outline-primary">View</button></td></tr>`
    )
  );
});

// responsive: collapse layout on small screens automatically
function adaptLayout() {
  if (window.innerWidth < 992) $("#mainLayout").addClass("collapsed");
  else $("#mainLayout").removeClass("collapsed");
}
adaptLayout();
$(window).on("resize", adaptLayout);
