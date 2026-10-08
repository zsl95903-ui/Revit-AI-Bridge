/* Revit AI console - self-contained UI for the AS.Tools.UI replacement.
 *
 * Host protocol (WebView2 message channel):
 *
 *   page -> host : window.chrome.webview.postMessage({ type, ... })
 *   host -> page : window.chrome.webview.addEventListener("message", ...)
 *
 *   page -> host                     host -> page
 *   -----------------------------    -----------------------------------
 *   settings.get                     ready            { tools }
 *   settings.save  {apiUrl,apiKey,model}
 *                                    settings         { apiUrl, model, hasKey }
 *                                    settings.saved   { message }
 *   settings.test  {apiUrl,apiKey,model}
 *                                    settings.testing { message }
 *                                    settings.ok      { response }
 *                                    settings.fail    { message }
 *   chat.send      { text }          chat.started     { }
 *   chat.cancel                      chat.status      { text }
 *   chat.reset                       chat.tool        { name, ok, summary }
 *   tools.list                       chat.assistant   { text }
 *                                    chat.error       { message }
 *                                    chat.done        { }
 */

const $ = id => document.getElementById(id);

const dom = {
  chip: $("chip"),
  log: $("log"),
  empty: $("empty"),
  input: $("input"),
  send: $("btn-send"),
  stop: $("btn-stop"),
  attach: $("btn-attach"),
  attachments: $("attachments"),
  tools: $("btn-tools"),
  settings: $("btn-settings"),
  modal: $("modal"),
  fUrl: $("f-url"),
  fKey: $("f-key"),
  fModel: $("f-model"),
  test: $("btn-test"),
  save: $("btn-save"),
  close: $("btn-close"),
  testOut: $("test-out"),
  drawer: $("drawer"),
  drawerClose: $("btn-drawer-close"),
  toolCount: $("tool-count"),
  toolFilter: $("tool-filter"),
  toolList: $("tool-list")
};

const state = {
  busy: false,
  tools: [],
  hostPresent: false,
  attachments: []
};

/* ---------------------------------------------------------------- transport */

function post(type, payload) {
  if (!window.chrome || !window.chrome.webview) {
    log("error", "未连接到 Revit 宿主（chrome.webview 不可用）。请从 Revit 的 Revit AI 面板打开。");
    return;
  }
  window.chrome.webview.postMessage(Object.assign({ type: type }, payload || {}));
}

function wire() {
  if (window.chrome && window.chrome.webview) {
    state.hostPresent = true;
    window.chrome.webview.addEventListener("message", onHostMessage);
    post("settings.get");
    post("tools.list");
  } else {
    state.hostPresent = false;
    setChip("无宿主", "bad");
    log("error", "此页面没有运行在 Revit 的 WebView2 里，无法与宿主通信。");
  }
}

/* ------------------------------------------------------------- host events */

function onHostMessage(event) {
  const d = event.data;
  if (!d || typeof d !== "object" || !d.type) return;

  switch (d.type) {
    case "attachment.picked":
      if (d.path) {
        state.attachments.push({ name: d.name || d.path, path: d.path, size: d.size || 0 });
        renderAttachments();
      }
      break;

    case "ready":
      setChip("就绪 · " + (d.tools || 0) + " 工具", "ok");
      break;

    case "settings":
      if (d.apiUrl) dom.fUrl.value = d.apiUrl;
      if (d.model) dom.fModel.value = d.model;
      dom.fKey.placeholder = d.hasKey ? "已保存（留空则沿用）" : "sk-...";
      break;

    case "settings.saved":
      setTest(d.message || "已保存。", "ok");
      break;

    case "settings.testing":
      setTest(d.message || "正在测试…", null);
      break;

    case "settings.ok":
      setTest("连接成功：" + (d.response || "OK"), "ok");
      break;

    case "settings.fail":
      setTest("连接失败：" + (d.message || ""), "fail");
      break;

    case "chat.started":
      state.busy = true;
      syncBusy();
      setChip("思考中…", "busy");
      clearStatus();
      status("已发送，等待模型…");
      break;

    case "chat.status":
      status(d.text || "");
      break;

    case "chat.tool":
      clearStatus();
      addTool(d.name, d.ok, d.summary);
      break;

    case "chat.assistant":
      clearStatus();
      log("assistant", d.text || "");
      break;

    case "chat.error":
      clearStatus();
      log("error", d.message || "未知错误");
      break;

    case "chat.done":
      state.busy = false;
      syncBusy();
      setChip("就绪 · " + state.tools.length + " 工具", "ok");
      break;

    case "tools":
      state.tools = Array.isArray(d.items) ? d.items : [];
      dom.toolCount.textContent = state.tools.length + " 个";
      renderTools();
      if (!state.busy) setChip("就绪 · " + state.tools.length + " 工具", "ok");
      break;

    default:
      break;
  }
}

/* ------------------------------------------------------------- chat render */

function log(kind, text) {
  if (dom.empty) dom.empty.classList.add("hidden");
  const el = document.createElement("div");
  el.className = "msg " + kind;
  el.textContent = text;
  dom.log.appendChild(el);
  dom.log.scrollTop = dom.log.scrollHeight;
  return el;
}

let statusEl = null;

function status(text) {
  if (!statusEl) {
    statusEl = log("status", text);
  } else {
    statusEl.textContent = text;
  }
  dom.log.scrollTop = dom.log.scrollHeight;
}

function clearStatus() {
  if (statusEl) {
    statusEl.remove();
    statusEl = null;
  }
}

function addTool(name, ok, summary) {
  if (dom.empty) dom.empty.classList.add("hidden");
  const box = document.createElement("div");
  box.className = "tool " + (ok ? "ok" : "fail");

  const head = document.createElement("div");
  const n = document.createElement("span");
  n.className = "name";
  n.textContent = name || "(未命名工具)";
  head.appendChild(n);
  const tag = document.createElement("span");
  tag.className = "c dim";
  tag.style.marginLeft = "6px";
  tag.textContent = ok ? "成功" : "失败";
  head.appendChild(tag);
  box.appendChild(head);

  if (summary) {
    const pre = document.createElement("pre");
    pre.textContent = String(summary).slice(0, 4000);
    box.appendChild(pre);
  }

  dom.log.appendChild(box);
  dom.log.scrollTop = dom.log.scrollHeight;
}

function renderTools() {
  const filter = (dom.toolFilter.value || "").trim().toLowerCase();
  dom.toolList.textContent = "";

  const items = state.tools.filter(t => {
    if (!filter) return true;
    return String(t.name || "").toLowerCase().includes(filter)
      || String(t.category || "").toLowerCase().includes(filter)
      || String(t.description || "").toLowerCase().includes(filter);
  });

  if (!items.length) {
    const p = document.createElement("div");
    p.className = "dim";
    p.textContent = state.tools.length ? "没有匹配的工具。" : "宿主尚未上报工具目录。";
    dom.toolList.appendChild(p);
    return;
  }

  for (const t of items) {
    const row = document.createElement("div");
    row.className = "item";

    const head = document.createElement("div");
    const n = document.createElement("span");
    n.className = "n";
    n.textContent = t.name || "?";
    head.appendChild(n);
    if (t.category) {
      const c = document.createElement("span");
      c.className = "c";
      c.textContent = t.category;
      head.appendChild(c);
    }
    row.appendChild(head);

    if (t.description) {
      const d = document.createElement("div");
      d.className = "d";
      d.textContent = t.description;
      row.appendChild(d);
    }

    dom.toolList.appendChild(row);
  }
}

/* ------------------------------------------------------------------ helpers */

function setChip(text, kind) {
  dom.chip.textContent = text;
  dom.chip.className = "chip" + (kind ? " " + kind : "");
}

function setTest(text, kind) {
  dom.testOut.textContent = text;
  dom.testOut.className = "output" + (kind ? " " + kind : "");
}

function syncBusy() {
  dom.send.disabled = state.busy;
  dom.stop.disabled = !state.busy;
  dom.input.disabled = state.busy;
}

function formValues() {
  return {
    apiUrl: dom.fUrl.value.trim(),
    apiKey: dom.fKey.value.trim(),
    model: dom.fModel.value.trim()
  };
}

/* ------------------------------------------------------------------- events */

dom.send.addEventListener("click", send);
dom.input.addEventListener("keydown", e => {
  if (e.key === "Enter" && !e.shiftKey) {
    e.preventDefault();
    send();
  }
});
dom.stop.addEventListener("click", () => post("chat.cancel"));
dom.attach.addEventListener("click", () => post("attachment.pick"));

function send() {
  if (state.busy) return;

  const text = dom.input.value.trim();
  const paths = state.attachments.map(a => a.path);
  if (!text && paths.length === 0) return;

  const shown = paths.length
    ? text + (text ? "\n\n" : "") + "[附件]\n" + paths.map(p => "- " + p).join("\n")
    : text;

  log("user", shown);
  dom.input.value = "";
  state.attachments = [];
  renderAttachments();
  post("chat.send", { text: shown });
  state.busy = true;
  syncBusy();
}

function renderAttachments() {
  dom.attachments.textContent = "";
  for (let i = 0; i < state.attachments.length; i++) {
    const a = state.attachments[i];
    const chip = document.createElement("span");
    chip.className = "file-chip";

    const label = document.createElement("span");
    label.textContent = a.name;
    label.title = a.path;
    chip.appendChild(label);

    const del = document.createElement("button");
    del.type = "button";
    del.textContent = "×";
    del.addEventListener("click", () => {
      state.attachments.splice(i, 1);
      renderAttachments();
    });
    chip.appendChild(del);

    dom.attachments.appendChild(chip);
  }
}

dom.settings.addEventListener("click", () => {
  dom.modal.classList.remove("hidden");
  setTest("", null);
  post("settings.get");
});
dom.close.addEventListener("click", () => dom.modal.classList.add("hidden"));
dom.modal.addEventListener("click", e => {
  if (e.target === dom.modal) dom.modal.classList.add("hidden");
});

dom.test.addEventListener("click", () => {
  setTest("正在测试…", null);
  dom.test.disabled = true;
  post("settings.test", formValues());
  setTimeout(() => { dom.test.disabled = false; }, 35000);
});

dom.save.addEventListener("click", () => post("settings.save", formValues()));

dom.tools.addEventListener("click", () => dom.drawer.classList.remove("hidden"));
dom.drawerClose.addEventListener("click", () => dom.drawer.classList.add("hidden"));
dom.toolFilter.addEventListener("input", renderTools);

wire();
syncBusy();
