const fs = require('fs').promises;
const path = require('path');

const dbPath = path.join(__dirname, 'ncs.json');

const NcModel = {
  getAll: async () => {
    try {
      try { await fs.access(dbPath); } catch { return []; }
      const data = await fs.readFile(dbPath, 'utf8');
      const allNcs = JSON.parse(data);
      
      // FILTER: Only show items that are NOT deleted
      // The deleted ones stay in the file for the auditor to see later
      return allNcs.filter(nc => !nc.deleted);
    } catch (error) {
      return [];
    }
  },

  create: async (ncData) => {
    let allNcs = [];
    try {
        const data = await fs.readFile(dbPath, 'utf8');
        allNcs = JSON.parse(data);
    } catch (e) {} // Handle empty file

    const novaNc = {
      id: Date.now(),
      dataCriacao: new Date().toISOString(),
      ...ncData,
      deleted: false // New items are active by default
    };

    allNcs.push(novaNc);
    await fs.writeFile(dbPath, JSON.stringify(allNcs, null, 2));
    return novaNc;
  },

  // --- UPGRADED "SOFT DELETE" ---
  delete: async (id, username) => {
    // 1. Read all data (including previously deleted stuff)
    const data = await fs.readFile(dbPath, 'utf8');
    let allNcs = JSON.parse(data);

    // 2. Find the item
    const index = allNcs.findIndex(nc => String(nc.id) === String(id));

    if (index !== -1) {
        // 3. Mark as deleted instead of removing
        allNcs[index].deleted = true;
        allNcs[index].deletedBy = username; // Track WHO
        allNcs[index].deletedAt = new Date().toISOString(); // Track WHEN
        
        // 4. Save back to file
        await fs.writeFile(dbPath, JSON.stringify(allNcs, null, 2));
    }
    return true;
  },

  // Add this inside the NcModel object
  getHistory: async () => {
    try {
        const data = await fs.readFile(dbPath, 'utf8');
        const allNcs = JSON.parse(data);
        // Return ONLY the deleted ones
        return allNcs.filter(nc => nc.deleted === true);
    } catch (error) {
        return [];
    }
}
};

module.exports = NcModel;