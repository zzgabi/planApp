﻿using ManageAccommodation.Models;
using ManageAccommodation.Models.DBObjects;
using ManageAccommodation.Repository;
using ManageAccommodation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ManageAccommodation.Controllers
{
    public class RoomController : Controller
    {
        private Repository.RoomRepository _repository;
        private Repository.DormRepository _dormRepository;
        private Repository.StudentRepository _studentRepository;

        public RoomController(ApplicationDbContext dbContext)
        {
            _repository = new RoomRepository(dbContext);
            _dormRepository = new DormRepository(dbContext);
            _studentRepository = new StudentRepository(dbContext);
        }


        // GET: RoomController
        public IActionResult Index(string sortOrder, int pg = 1)
        {
            var rooms = _repository.GetAllRoomsInfo();

            //pager logic
            const int pageSize = 10;

            if (pg < 1)
                pg = 1;

            int recsCount = rooms.Count();

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var pageRooms = rooms.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;

            foreach (var item in rooms)
            {
                item.RoomNo = item.Idroom.ToString().Substring(0, 3);
                item.DormName = _dormRepository.GetDormByID(item.Iddorm).DormName;
            }

            ViewData["VacancySortParm"] = String.IsNullOrEmpty(sortOrder) ? "Ocupied" : "";
            switch (sortOrder)
            {
                case "Ocupied":
                    pageRooms = rooms.OrderBy(x => x.Status).ToList();
                    break;
                default:
                    pageRooms = rooms.OrderByDescending(x => x.Status).ToList();
                    break;
            }
            return View("Index", pageRooms);
        }
       
        // GET: RoomController/Details/5
        public ActionResult Details(Guid id)
        {
            var model = _repository.GetRoomById(id);
            model.RoomNo = id.ToString().Substring(0, 3);
            model.DormName = _dormRepository.GetDormByID(model.Iddorm).DormName;
            

            var studentModel = _studentRepository.GetStudentsByIdRoom(id);
            ViewData["StudsAssign"] = studentModel;
            return View("RoomDetails", model);



        }

        [Authorize(Roles = "KeyRole, Admin")]
        // GET: RoomController/Create
        public  ActionResult Create()
        {
            var dormsList = _dormRepository.GetAllDormsInfo().Select(x => new SelectListItem(x.DormName, x.Iddorm.ToString()));
            ViewBag.DormList = dormsList;

            List<SelectListItem> Status = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Vacancy", Value="Vacancy"},
                new SelectListItem() { Text="Ocupied", Value="Ocupied"},
            };
            ViewBag.Status = Status;
            return View("CreateRoom");
        }

        // POST: RoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                //Models.RoomModel model = new Models.RoomModel();
                var model = new RoomModel();
                var task = TryUpdateModelAsync(model);
                task.Wait();

                _repository.InsertRoom(model);

                if (task.Result)
                {
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View("CreateRoom");
            }
        }
        [Authorize(Roles = "KeyRole, Admin")]
        // GET: RoomController/Edit/5
        public ActionResult Edit(Guid id)
        {
            var dormsList = _dormRepository.GetAllDormsInfo().Select(x => new SelectListItem(x.DormName, x.Iddorm.ToString()));
            ViewBag.DormList = dormsList;


            var model = _repository.GetRoomById(id);
            return View("EditRoom", model);
        }

        // POST: RoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, IFormCollection collection)
        {
            try
            {
                var model = new RoomModel();

                var task = TryUpdateModelAsync(model);
                task.Wait();

                _repository.UpdateRoom(model);
                return RedirectToAction("Index");

            }
            catch
            {
                return RedirectToAction("Index", id);
            }
        }
        [Authorize(Roles = "Admin")]
        // GET: RoomController/Delete/5
        public ActionResult Delete(Guid id)
        {
            var model = _repository.GetRoomById(id);
            return View("DeleteRoom", model);
        }

        // POST: RoomController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var model = _repository.GetRoomById(id);
                _repository.DeleteRoom(model);
                return RedirectToAction("Index");
            }
            catch
            {
                return View("DeleteRoom", id);
            }
        }
    }
}
