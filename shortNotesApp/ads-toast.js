// shortNotesApp/Scripts/add-toast.js
// Fetch ads from the server and show them using native alert()/confirm() dialogs (no HTML toasts).
// - Tries /shortNotesApp/GetAds.ashx first, then /shortNotesApp/GetNewSubjects.ashx as a fallback.
// - Tracks dismissed/seen ads in localStorage under key 'seenAds_v1' so they don't show again.
// - After alert() the user is prompted with confirm() to "Open" the ad (OK = Open, Cancel = Dismiss).
// - If ad has a 'link' property it will be opened; otherwise it attempts a default ShortNotes<TitleWithoutSpaces>.aspx route.

(function () {
  const ENDPOINTS = [
    'GetAds.ashx'
  ];
  const SEEN_KEY = 'seenAds_v1';

  function getSeenSet() {
    try {
      const raw = localStorage.getItem(SEEN_KEY);
      if (!raw) return new Set();
      const arr = JSON.parse(raw);
      return new Set(Array.isArray(arr) ? arr : []);
    } catch (e) {
      console.warn('add-toast:getSeenSet parse error', e);
      return new Set();
    }
  }

  function saveSeenSet(set) {
    try {
      localStorage.setItem(SEEN_KEY, JSON.stringify(Array.from(set)));
    } catch (e) {
      console.warn('add-toast:saveSeenSet', e);
    }
  }

  function markSeen(id) {
    const s = getSeenSet();
    s.add(Number(id));
    saveSeenSet(s);
  }

  // Try endpoints sequentially until we get an array response
  async function fetchAdsFromEndpoints() {
    for (let i = 0; i < ENDPOINTS.length; i++) {
      const url = ENDPOINTS[i];
      try {
        const res = await fetch(url, { cache: 'no-store' });
        if (!res.ok) {
          // If 404 on first, try next
          console.debug(`add-toast: fetch ${url} returned ${res.status}`);
          continue;
        }
        const json = await res.json();
        // If endpoint returned an error object, skip it
        if (!Array.isArray(json)) {
          if (json && json.error) {
            console.warn('add-toast: endpoint returned error', json.message || json);
            continue;
          }
          // Not array and not error: skip
          continue;
        }
        return json;
      } catch (err) {
        console.warn('add-toast: fetch failed for', url, err);
        // try next endpoint
      }
    }
    return [];
  }

  // Sequentially show alerts for unseen ads.
  async function showAlertsSequentially(ads) {
    if (!Array.isArray(ads) || ads.length === 0) return;
    // Sort newest first so user sees latest: can change to oldest-first if desired
    ads.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
    const seen = getSeenSet();
    for (const ad of ads) {
      if (seen.has(ad.id)) continue; // skip already seen
      // Build message shown in alert
      const title = ad.title || ad.name || 'Announcement';
      const msg = ad.message || ad.body || '';
      const full = title + '\n\n' + (msg ? msg + '\n\n' : '') + (ad.createdAt ? `Added: ${new Date(ad.createdAt).toLocaleString()}` : '');

      try {
        // show main alert (blocking)
        alert(full);
      } catch (e) {
        // in case alert is unavailable for some reason
        console.log('add-toast: alert failed', e);
      }

      // After alert, prompt whether to open the ad link (OK = Open, Cancel = Dismiss)
 /*     let open = false;
      try {
        open = confirm('Open this announcement now? (OK = Open, Cancel = Dismiss)');
      } catch (e) {
        console.log('add-toast: confirm failed', e);
      }*/

      // mark seen in either case so it doesn't appear again
      markSeen(ad.id);

/*      if (open) {
        // if ad.link present, use it; otherwise try default ShortNotes<TitleNoSpaces>.aspx
        const link = (ad.link && typeof ad.link === 'string' && ad.link.length) ? ad.link : null;
        if (link) {
          // allow relative or absolute links
          window.location.href = link;
          // stop further alerts (navigating away)
          return;
        } else {
          const titleForUrl = (ad.title || ad.name || '').replace(/\s+/g, '');
          const target = `/shortNotesApp/ShortNotes${titleForUrl}.aspx`;
          window.location.href = target;
          return;
        }
      }*/

      // if dismissed (user cancelled confirm), continue to next ad
    }
  }

  // Entry point
  document.addEventListener('DOMContentLoaded', async function () {
    const ads = await fetchAdsFromEndpoints();
    if (!Array.isArray(ads) || ads.length === 0) return;
    await showAlertsSequentially(ads);
  });
})();