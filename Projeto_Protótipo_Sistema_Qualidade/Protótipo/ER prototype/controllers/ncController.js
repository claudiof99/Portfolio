const NcModel = require('../models/NcModel');

const ncController = {

  // 1. Create New NC
  novaNc: async (req, res) => {
    try {
      const { titulo, descricao, gravidade } = req.body;
      
      // Handle Image Path
      let caminhoImagem = null;
      if (req.file) {
        // Fix path for browser (remove 'public')
        caminhoImagem = '/uploads/' + req.file.filename;
      }

      if (!titulo || !descricao) {
        return res.send("Error: Title and Description are required.");
      }

      await NcModel.create({ 
        titulo, 
        descricao, 
        gravidade, 
        evidencia: caminhoImagem 
      });

      res.redirect('/ncs');

    } catch (error) {
      console.error(error);
      res.status(500).send("Error saving NC");
    }
  },

  // 2. List NCs
  listarNcs: async (req, res) => {
    try {
      const ncs = await NcModel.getAll();
      // Pass currentUser so we can check permissions in the view
      res.render('lista-ncs', { ncs: ncs, currentUser: req.session.user }); 
    } catch (error) {
      console.error(error);
      res.status(500).send("Error fetching NCs");
    }
  },



 // 3. UPDATED DELETE FUNCTION 
  deleteNc: async (req, res) => {
    try {
        const id = req.params.id;
        
       
        const user = req.session.user;
        const recordedName = user.username || user.email || user.role || 'Admin';

        await NcModel.delete(id, recordedName);
        
        res.redirect('/ncs');
    } catch (error) {
        console.error(error);
        res.status(500).send("Error deleting NC");
    }
  },
  
  // 4. History (Keep this as is)
  history: async (req, res) => {
      try {
          const deletedNcs = await NcModel.getHistory();
          res.render('history-ncs', { 
              ncs: deletedNcs, 
              currentUser: req.session.user 
          });
      } catch (error) {
          console.error(error);
          res.status(500).send("Error fetching history");
      }
  }
};

module.exports = ncController;