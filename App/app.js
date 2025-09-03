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

function renderTables() {
  // Reports
  const $tbl = $("#tblReports").empty();
  state.reports.forEach((r) => {
    $tbl.append(`<tr>
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
          <td>
            <button class="btn btn-sm btn-outline-primary action-btn view-report" data-id="${
              r.id
            }"><i class="fa-solid fa-eye"></i></button>
            <button class="btn btn-sm btn-outline-secondary action-btn assign-report" data-id="${
              r.id
            }"><i class="fa-solid fa-user-plus"></i></button>
          </td>
        </tr>`);
  });

  // Investigations
  const $inv = $("#tblInvestigations").empty();
  state.investigations.forEach((i) => {
    $inv.append(
      `<tr><td>${i.id}</td><td>${i.caseId}</td><td>${escape(
        i.lead
      )}</td><td>${escape(i.stage)}</td><td>${i.updated || ""}</td></tr>`
    );
  });

  // Evidence
  const $e = $("#tblEvidence").empty();
  state.evidence.forEach((e) => {
    $e.append(
      `<tr><td>${e.id}</td><td>${escape(e.type)}</td><td>${escape(
        e.desc
      )}</td><td>${e.caseId || ""}</td><td>${escape(
        e.location || ""
      )}</td><td><button class="btn btn-sm btn-light"><i class="fa-solid fa-download"></i></button></td></tr>`
    );
  });

  // Criminals
  const $c = $("#tblCriminals").empty();
  state.criminals.forEach((c) => {
    $c.append(
      `<tr><td>${c.id}</td><td>${escape(c.name)}</td><td>${escape(
        c.aliases
      )}</td><td>${escape(c.risk)}</td><td>${escape(
        c.lastSeen
      )}</td><td><button class="btn btn-sm btn-outline-secondary">View</button></td></tr>`
    );
  });

  // Users
  const $u = $("#tblUsers").empty();
  state.users.forEach((u) => {
    $u.append(
      `<tr><td>${u.uid}</td><td>${escape(u.name)}</td><td>${escape(
        u.role
      )}</td><td>${escape(u.station)}</td><td>${escape(
        u.lastLogin
      )}</td><td><button class="btn btn-sm btn-outline-secondary">Edit</button></td></tr>`
    );
  });

  // Logs
  const $l = $("#tblLogs").empty();
  state.logs.forEach((log) => {
    $l.append(
      `<tr><td>${escape(log.time)}</td><td>${escape(log.user)}</td><td>${escape(
        log.action
      )}</td><td>${escape(log.details)}</td></tr>`
    );
  });

  // Notifications
  const $n = $("#notifList").empty();
  state.notifications.forEach((n) => {
    $n.append(`<li class="list-group-item d-flex justify-content-between align-items-center">
          <div>
            <div>${escape(n.message)}</div>
            <small class="muted">${escape(n.time)}</small>
          </div>
          <div><button class="btn btn-sm btn-outline-success mark-read" data-id="${
            n.id
          }"><i class="fa-solid fa-check"></i></button></div>
        </li>`);
  });
  $("#notifstatus_badge").text(state.notifications.length);
}

function escape(s) {
  return s === null || s === undefined
    ? ""
    : String(s).replace(
        /[&<>]/g,
        (c) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;" }[c])
      );
}

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
  seed();
  renderAll();

  $(".topbar").hide();
  $("#mainLayout").hide();
  $("#view-login").show();
  $("#loginPassword").val("");
  $("#loginUsername").val("");

  // show view
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

  // sidebar clicks
  $(".nav-link").on("click", function (e) {
    e.preventDefault();
    const v = $(this).data("view");
    if (v) showView(v);
  });

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
  $("#formNewReport").on("submit", function (e) {
    e.preventDefault();
    const data = Object.fromEntries(new FormData(this).entries());
    const rec = {
      id: genId("RPT"),
      title: data.title || "Untitled",
      type: data.type || "other",
      location: data.location || "-",
      reported: new Date().toISOString().slice(0, 10),
      status: "Open",
    };
    state.reports.unshift(rec);
    renderAll();
    bootstrap.Modal.getInstance(
      document.getElementById("modalNewReport")
    ).hide();

    // TODO: Replace this block with AJAX to your .NET Core API:
    // $.ajax({ url:'/api/reports', method:'POST', data: JSON.stringify(rec), contentType:'application/json' })
  });

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
    bootstrap.Modal.getInstance(
      document.getElementById("modalEvidence")
    ).hide();
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
  $("#log_out_page").on("click", function () {
    $(".topbar").hide();
    $("#mainLayout").hide();
    $("#view-login").show();
  });
  $("#loginForm").on("submit", function (e) {
    e.preventDefault();

    let user = $("#loginUsername").val().trim();
    let pass = $("#loginPassword").val().trim();

    if (user === "Admin" && pass === "1234") {
      // Hide login, show dashboard layout
      $("#view-login").hide();
      $(".topba").show();
      $("#mainLayout").show();
      $("#loginPassword").val("");
      $("#loginUsername").val("");

      showView("dashboard");

      
    } else {
      $("#loginError").fadeIn();
    }
  });
  // responsive: collapse layout on small screens automatically
  function adaptLayout() {
    if (window.innerWidth < 992) $("#mainLayout").addClass("collapsed");
    else $("#mainLayout").removeClass("collapsed");
  }
  adaptLayout();
  $(window).on("resize", adaptLayout);
});

function renderAll() {
  renderKPIs();
  renderRecentReports();
  renderTables();
  renderChart();
}
