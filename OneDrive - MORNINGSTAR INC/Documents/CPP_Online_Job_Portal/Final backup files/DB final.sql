Drop table Admin;
Drop table Users;
Drop table jobpostings;
Drop table jobseeker;
Drop table recruiters;
Drop table JobApplication;

--------------- Admin Table --------------------------
CREATE TABLE Admin (
    Admin_Id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY(start with 10000 increment by 1),
    Username VARCHAR(255) UNIQUE,
    password Varchar(255) NOT NULL
);

-- Insert Sample Data into Admin Table
INSERT INTO Admin (Username, password)
VALUES
    ('vikas_admin', '10000A1'),
    ('priya_admin', '10001A2'),
    ('rohit_admin', '10002A3'),
    ('sneha_admin', '10003A4');

select * from Admin;

-- Function to retrieve username using Admin_Id
CREATE OR REPLACE FUNCTION get_username_by_admin(p_Username INT)
RETURNS VARCHAR(255)
LANGUAGE plpgsql
AS $$
DECLARE
    v_username VARCHAR(255);
BEGIN
    -- Retrieve the username where Admin_Id matches
    SELECT Username INTO v_username
    FROM Admin
    WHERE Username = p_Username;

    RETURN v_username;
END;
$$; 

SELECT get_username_by_admin_id(10000);

----------------------------
CREATE OR REPLACE FUNCTION getalladmin()
RETURNS TABLE (Admin_Id Int, Username VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT a.Admin_Id, a.Username
    FROM admin a;
END;
$$;

-- Example call to get the jobseeker with ID 3000
SELECT * FROM getalladmin()


-- Stored Procedure to insert a new Admin
CREATE OR REPLACE PROCEDURE insert_admin(
    p_username VARCHAR(255),
    p_password VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Insert new admin into the Admin table
    INSERT INTO Admin (Username, Password)
    VALUES (p_username, p_password);
END;
$$;

-- Example of calling the stored procedure
CALL insert_admin('new_admin', 'secure_password123');


-------------------- Job seeker Table ---------------------
CREATE TABLE Jobseeker (
    jobseeker_id int PRIMARY KEY GENERATED ALWAYS AS IDENTITY(start with 3000 increment by 1),
    name VARCHAR NOT NULL,
	password VARCHAR NOT NULL,
    email VARCHAR UNIQUE NOT NULL,
    phone_number VARCHAR UNIQUE,
    address VARCHAR,
	designation TEXT not null,
    education_bg TEXT,
    work_experience TEXT,
    skills TEXT,
    resume_link VARCHAR,   
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


----- Insert Sample Data into Jobseeker Table

INSERT INTO Jobseeker (name, password,email, phone_number, address, designation, education_bg, work_experience, skills, resume_link)
VALUES
('Rohit Kumar', 'password123','rohit.kumar@example.com', '9123456789', '12, A Block, Sector 15, Noida', 'Software Engineer', 'BTech in Computer Science from IIT Delhi', '2 years at TechSolutions Ltd. as a Software Developer', 'Java, Python, SQL', 'http://example.com/resume/rohitkumar'),
('Surya Veer','12345678', 'surya.veer@example.com', '9123446789', '34, B Street, Indira Nagar, Bangalore', 'Data Analyst', 'MSc in Data Science from Bangalore University', '3 years at DataCorp as a Data Analyst', 'Python, R, Tableau', 'http://example.com/resume/suryaveer'),
('Amit Shah', 'pas123','amit.shah@example.com', '9113446789', '56, C Lane, Ghatkopar, Mumbai', 'Graphic Designer', 'BA in Design from National Institute of Design', '4 years at Creative Solutions as a Senior Graphic Designer', 'Adobe Illustrator, Photoshop, InDesign', 'http://example.com/resume/amitshah'),
( 'Ajay Sharma', 'password','ajay.sharma@example.com', '9123436789', '78, D Road, Rajajinagar, Bangalore', 'Project Manager', 'MBA from Indian Institute of Management Bangalore', '5 years at ProjectWorks Ltd. as a Project Manager', 'Agile, Scrum, Project Planning', 'http://example.com/resume/ajaysharma'),
( 'Veer Phariya','P@ssw0rd!2024' ,'veer.phariya@example.com', '9238776655', '89, E Street, Malad, Mumbai', 'Marketing Manager', 'MBA in Marketing from Mumbai University', '6 years at MarketGurus as a Marketing Manager', 'Digital Marketing, Strategy, Analytics', 'http://example.com/resume/veerphariya'),
( 'Seema Kumar', 'S3cure' , 'seema.kumar@example.com', '9028776655', '101, F Street, Chandigarh', 'HR Manager', 'MBA in Human Resources from IIM Ahmedabad', '7 years at HR Solutions as an HR Manager', 'Recruitment, Employee Relations, Compensation', 'http://example.com/resume/seemakumar'),
( 'Ravi Patel', '@Pass' , 'ravi.patel@example.com', '9991234567', '202, G Lane, Navi Mumbai', 'Content Writer', 'MA in English Literature from Mumbai University', '4 years at ContentMasters as a Senior Content Writer', 'Creative Writing, SEO, Copywriting', 'http://example.com/resume/ravipatel');


select * from jobseeker
select * from recruiters

---------------- JOb seeker Indexxes -------------------------------
--index creation for job seeker designation column
CREATE INDEX idx_jobseeker_designation
ON Jobseeker (designation);

--index for job seeker designation column
SELECT * 
FROM pg_indexes 
WHERE tablename = 'Jobseeker' 
  AND indexname = 'idx_jobseeker_designation';


EXPLAIN ANALYZE
SELECT * 
FROM Jobseeker
WHERE designation = 'Data Analyst';


----Stored procedure for Jobseeker table
CREATE OR REPLACE FUNCTION getJobseekerById(p_jobseeker_id INT)
RETURNS TABLE (jobseeker_id INT, name VARCHAR, email VARCHAR, phone_number VARCHAR, address VARCHAR, designation TEXT, education_bg TEXT, work_experience TEXT, skills TEXT, resume_link VARCHAR, created_at TIMESTAMP)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT js.jobseeker_id, js.name, js.email, js.phone_number, js.address, js.designation, 
           js.education_bg, js.work_experience, js.skills, js.resume_link, js.created_at
    FROM Jobseeker js
    WHERE js.jobseeker_id = p_jobseeker_id;
END;
$$;

-- Example call to get the jobseeker with ID 3000
SELECT * FROM getJobseekerById(3000);

 
----------------------------------------------------

CREATE OR REPLACE PROCEDURE insertJobseeker( p_name VARCHAR, p_email VARCHAR, p_phone VARCHAR, 
 p_addr TEXT, p_desig TEXT, p_edu TEXT, p_work TEXT, p_skills TEXT, p_resume VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO Jobseeker (name, email, phone_number, address, designation, 
	         education_bg, work_experience,  skills,  resume_link)  
    VALUES (p_name, p_email, p_phone, p_addr, p_desig, p_edu, p_work, 
	        p_skills, p_resume);   
EXCEPTION
    WHEN unique_violation THEN
        RAISE NOTICE 'Jobseeker ID % already exists.', p_id;
    WHEN others THEN
        RAISE NOTICE 'Error inserting jobseeker: %', SQLERRM;
END;
$$;

CALL insertJobseeker(
    'John Doe',        -- p_name
    'john.doe@example.com', -- p_email
    '123-456-7890',    -- p_phone
    '123 Main St, Anytown, USA', -- p_addr
    'Software Engineer', -- p_desig
    'Bachelor of Science in Computer Science', -- p_edu
    '3 years at XYZ Corp as a Software Developer', -- p_work
    'C#, SQL, JavaScript', -- p_skills
    'http://example.com/resume/john_doe'
);

---------------------------

CREATE OR REPLACE PROCEDURE updateJobseeker(p_jobseeker_id INT,p_email VARCHAR, p_phone VARCHAR, 
 p_addr TEXT, p_desig TEXT, p_edu TEXT, p_work TEXT, p_skills TEXT, p_resume VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Jobseeker
    SET 
        email = p_email,
        phone_number = p_phone,
        address = p_addr,
		designation = p_desig,
        education_bg = p_edu,
		work_experience = p_work,
		skills = p_skills,
		resume_link = p_resume
    WHERE jobseeker_id = p_jobseeker_id; 
END;
$$;

call updateJobseeker(3001,
    'surya@example.com', 
    '987-654-3210', 
    '456 Oak Avenue', 
	'Software Engineer',
    'MSc in Computer Science', '2 years in Masters and 4 years in BTech','Java, Python, SQL', 'http://example.com/resume/surya')
-------------------------------------------

CREATE OR REPLACE PROCEDURE deleteJobseeker(p_jobseeker_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    
    DELETE FROM Jobseeker
    WHERE jobseeker_id = p_jobseeker_id;

    IF NOT FOUND THEN
        RAISE NOTICE 'No jobseeker found with ID %', p_jobseeker_id;
    ELSE
        RAISE NOTICE 'Jobseeker with ID % has been successfully deleted.', p_jobseeker_id;
    END IF;
EXCEPTION
    WHEN others THEN 
        RAISE NOTICE 'An error occurred while deleting jobseeker record: %', SQLERRM;
END;
$$;

CALL deleteJobseeker(3006);
SELECT * FROM JOBSEEKER


DROP PROCEDURE IF EXISTS updateJobseeker( p_uid INT, p_email VARCHAR, p_phone VARCHAR, 
 p_addr TEXT, p_desig VARCHAR, p_edu TEXT, p_work TEXT, p_skills TEXT, p_resume VARCHAR)
 
CALL insertJobseeker(101, 'John Doe', 'john.doe@example.com', '9123456780', '1, B Street, City Center, Bangalore', 'Software Developer', 
'MSc in Software Engineering from XYZ University', '3 years at ABC Corp as a Software Developer', 'Java, C++, SQL', 'http://example.com/resume/johndoe', '2024-09-07 19:18:54');


CREATE OR REPLACE PROCEDURE registerJobseeker( p_name VARCHAR, p_pass VARCHAR, p_email VARCHAR,  p_desig TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO Jobseeker (name, password, email, designation)  
    VALUES (p_name, p_pass, p_email, p_desig);   
EXCEPTION
    WHEN unique_violation THEN
        RAISE NOTICE 'Jobseeker ID % already exists.', p_email;
    WHEN others THEN
        RAISE NOTICE 'Error inserting jobseeker: %', SQLERRM;
END;
$$;
 
CALL registerJobseeker(
    'Sane Doe',
    'securepass123',
    'j.doess@example.com',
    'Web Developer'
);


----------
SELECT * FROM getJobseekerByEmailId('surya.veer@example.com');

CREATE OR REPLACE FUNCTION getJobseekerByEmailId(p_email VARCHAR)
RETURNS TABLE (
    jobseeker_id INT,
    name VARCHAR,
    email VARCHAR,
	password VARCHAR,
    phone_number VARCHAR,
    address VARCHAR,
    designation TEXT,
    education_bg TEXT,
    work_experience TEXT,
    skills TEXT,
    resume_link VARCHAR,
    created_at TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT js.jobseeker_id, js.name, js.email, js.password,js.phone_number, js.address, js.designation, 
           js.education_bg, js.work_experience, js.skills, js.resume_link, js.created_at
    FROM Jobseeker js
    WHERE js.email = p_email;
END;
$$;


------------------ Recurtiers table -----------------------
CREATE TABLE Recruiters (
    recruiter_id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY(start with 1000 increment by 1),
    company_name VARCHAR(255) unique NOT NULL,
    location VARCHAR(255),
    contact_number VARCHAR(15),
    email VARCHAR(255) UNIQUE NOT NULL,
	password VARCHAR(255) NOT NULL
);


-- Insert Sample Data into Recruiters Table
INSERT INTO Recruiters (company_name, location, contact_number, email, password)
VALUES
    ('TechWizards Pvt Ltd', 'Bengaluru', '9876543210', 'contact@techwizards.com','Tech@123'),
    ('InfoSphere Solutions', 'Mumbai', '9988776655', 'info@infosphere.com','Info@123'),
    ('QuantumSoft Technologies', 'Hyderabad', '9123456789', 'support@quantumsoft.com','Quantum@123'),
    ('AlphaBytes', 'Chennai', '9765432109', 'hello@alphabytes.com','alpha@123'),
    ('NextGen Innovators', 'Pune', '9988221100', 'contact@nextgeninnovators.com','next@123');

select * from Recruiters;

-- Function to retrieve recruiter details using recruiter_id
CREATE OR REPLACE FUNCTION get_recruiter_by_email(p_email varchar)
RETURNS TABLE (
    company_name VARCHAR(255),
    location VARCHAR(255),
    contact_number VARCHAR(15),
    email VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Retrieve data from Recruiters where recruiter_id matches the input
    RETURN QUERY
    SELECT
        r.company_name,
        r.location,
        r.contact_number,
        r.email
    FROM
        Recruiters r
    WHERE
        r.email = p_email;
END;
$$;

SELECT * FROM get_recruiter_by_email('contact@techwizards.com');



-- Stored Procedure to Insert Data into Recruiters Table
CREATE OR REPLACE PROCEDURE insert_recruiter(p_company_name VARCHAR, p_location VARCHAR, p_contact_number VARCHAR, p_email VARCHAR, p_password VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Insert new recruiter record
    INSERT INTO Recruiters (company_name, location, contact_number, email, password)
    VALUES (p_company_name, p_location, p_contact_number, p_email,p_password);

    -- Inform about successful insertion
    RAISE NOTICE 'New recruiter % has been successfully inserted.', p_company_name;
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions and provide a message
        RAISE EXCEPTION 'An error occurred while inserting new recruiter %: %', p_company_name, SQLERRM;
END;
$$;

CALL insert_recruiter('NewTech Solutions', 'Delhi', '9988776655', 'contact@newtech.com','new@123');

--------- Update recruiter
CREATE OR REPLACE PROCEDURE update_recruiters(
    p_company_name VARCHAR,
    p_location VARCHAR,
    p_contact_number VARCHAR,
    p_email VARCHAR,
	p_password VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Recruiters
    SET 
        company_name = p_company_name,
        location = p_location,
		contact_number = p_contact_number,
        password = p_password
    WHERE email = p_email;
END;
$$;

call update_recruiters('XYZ corp','Lucknow','9765432109','hello@alphabytes.com','abc@123')

-- Create Stored Procedure to Update Location
CREATE OR REPLACE PROCEDURE update_recruiter_location(p_recruiter_id INT, p_location VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Check if the recruiter ID exists before updating
    IF EXISTS (SELECT 1 FROM Recruiters WHERE recruiter_id = p_recruiter_id) THEN
        -- Update the location of the recruiter
        UPDATE Recruiters
        SET location = p_location
        WHERE recruiter_id = p_recruiter_id;

        -- Inform about successful update
        RAISE NOTICE 'Location for recruiter with ID % has been updated to %.', p_recruiter_id, p_location;
    ELSE
        -- Inform if the recruiter ID does not exist
        RAISE NOTICE 'Recruiter with ID % does not exist.', p_recruiter_id;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions and provide a message
        RAISE EXCEPTION 'An error occurred while updating location for recruiter with ID %: %', p_recruiter_id, SQLERRM;
END;
$$;

CALL update_recruiter_location(1000, 'Lucknow');


-- Create Stored Procedure to Update Contact Number
CREATE OR REPLACE PROCEDURE update_recruiter_contact_number(p_recruiter_id INT, p_contact_number VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Check if the recruiter ID exists before updating
    IF EXISTS (SELECT 1 FROM Recruiters WHERE recruiter_id = p_recruiter_id) THEN
        -- Update the contact number of the recruiter
        UPDATE Recruiters
        SET contact_number = p_contact_number
        WHERE recruiter_id = p_recruiter_id;

        -- Inform about successful update
        RAISE NOTICE 'Contact number for recruiter with ID % has been updated to %.', p_recruiter_id, p_contact_number;
    ELSE
        -- Inform if the recruiter ID does not exist
        RAISE NOTICE 'Recruiter with ID % does not exist.', p_recruiter_id;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions and provide a message
        RAISE EXCEPTION 'An error occurred while updating contact number for recruiter with ID %: %', p_recruiter_id, SQLERRM;
END;
$$;

CALL update_recruiter_contact_number(1001, '9133781289');

-- Create Stored Procedure to Update Email
CREATE OR REPLACE PROCEDURE update_recruiter_email(p_recruiter_id INT, p_email VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Check if the recruiter ID exists before updating
    IF EXISTS (SELECT 1 FROM Recruiters WHERE recruiter_id = p_recruiter_id) THEN
        -- Update the email of the recruiter
        UPDATE Recruiters
        SET email = p_email
        WHERE recruiter_id = p_recruiter_id;

        -- Inform about successful update
        RAISE NOTICE 'Email for recruiter with ID % has been updated to %.', p_recruiter_id, p_email;
    ELSE
        -- Inform if the recruiter ID does not exist
        RAISE NOTICE 'Recruiter with ID % does not exist.', p_recruiter_id;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions and provide a message
        RAISE EXCEPTION 'An error occurred while updating email for recruiter with ID %: %', p_recruiter_id, SQLERRM;
END;
$$;

CALL update_recruiter_email(1002, 'newemail@example.com');


-- Create Stored Procedure to Delete Recruiter
CREATE OR REPLACE PROCEDURE delete_recruiter_by_email(p_email varchar)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Check if the recruiter ID exists before attempting to delete
    IF EXISTS (SELECT 1 FROM Recruiters WHERE email = p_email) THEN
        -- Delete the recruiter record
        DELETE FROM Recruiters
        WHERE email = p_email;
        
        -- Optional: Inform about successful deletion
        RAISE NOTICE 'Recruiter with email % has been successfully deleted.', p_email;
    ELSE
        -- Inform if the recruiter ID does not exist
        RAISE NOTICE 'Recruiter with email % does not exist.', p_email;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions and provide a message
        RAISE EXCEPTION 'An error occurred while deleting recruiter with ID %: %', p_email, SQLERRM;
END;
$$;

CALL delete_recruiter_by_email('contact@newtech.com');

select * from recruiters
-------------- JobPostings Table ---------------------
CREATE TABLE JobPostings (
    job_id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY(start with 2000 increment by 1),
    job_title VARCHAR(255) unique NOT NULL,
    job_location VARCHAR(255) NOT NULL, 
    contact_person VARCHAR(255) NOT NULL,
    contact_number VARCHAR(15) NOT NULL,
	functional_skills TEXT,
    technical_skills TEXT, 
    job_description TEXT unique NOT NULL,
    job_type VARCHAR(50),
    post_date DATE DEFAULT CURRENT_TIMESTAMP,
    last_date date check(last_date >= current_date),
    salary MONEY,
	remail VARCHAR(50),
	jp_companyname VARCHAR(255) REFERENCES Recruiters(company_name) ON DELETE CASCADE,
    CONSTRAINT FK_Recruiter FOREIGN KEY (remail) REFERENCES Recruiters(email) ON DELETE CASCADE
);

-- Insert Sample Data into JobPosting Table
INSERT INTO JobPostings (job_title, job_location, contact_person, contact_number, functional_skills, technical_skills, job_description, job_type, last_date, salary, remail, jp_companyname)
VALUES 
('Software Developer', 'Noida, Uttar Pradesh', 'Anita Patel', '9988776655', 'Teamwork, Communication', 'Java, Python, SQL', 'Develop and maintain software applications. Collaborate with other teams to define project requirements and deliver solutions.', 'Full-time', '2025-10-15', '80000', 'contact@techwizards.com','TechWizards Pvt Ltd'),
('Senior Graphic Designer', 'Mumbai, Maharashtra', 'Amit Shah', '9113446789', 'Creativity, Time Management', 'Adobe Illustrator, Photoshop, InDesign', 'Design marketing materials including brochures, flyers, and digital ads. Collaborate with the marketing team to develop creative concepts.', 'Contract', '2025-11-01', '60000', 'info@infosphere.com','InfoSphere Solutions'),
('Data Analyst', 'Bangalore, Karnataka', 'Sneha Rao', '9123446789', 'Analytical Thinking, Attention to Detail', 'Python, R, Tableau', 'Analyze and interpret complex data sets. Create reports and dashboards to support business decision-making.', 'Full-time', '2025-12-01', '75000', 'support@quantumsoft.com','QuantumSoft Technologies'),
('HR Manager', 'Chandigarh', 'Seema Kumar', '9028776655', 'Communication, Conflict Resolution', 'Recruitment, Employee Relations', 'Manage HR functions including recruitment, employee relations, and performance management. Develop and implement HR policies.', 'Full-time', '2025-10-30', '90000', 'hello@alphabytes.com','AlphaBytes'),
('Marketing Manager', 'Mumbai, Maharashtra', 'Veer Phariya', '9238776655', 'Strategic Thinking, Leadership', 'Digital Marketing, Market Research', 'Plan and execute marketing strategies. Oversee market research and manage the marketing team to drive company growth.', 'Full-time', '2025-11-15', '85000', 'contact@techwizards.com','TechWizards Pvt Ltd'),
('Product Designer', 'Mumbai, Maharashtra', 'Amit Shah', '9113446789', 'Design Thinking, User Research', 'Sketch, Figma', 'Design user-centered products and interfaces. Conduct user research and collaborate with development teams to deliver high-quality designs.', 'Contract', '2025-10-30', '65000', 'info@infosphere.com','InfoSphere Solutions');

select * from jobpostings;

-- Function to Retrieve Data from JobPostings by Job ID
CREATE OR REPLACE FUNCTION get_job_posting_by_remail(p_remail varchar)
RETURNS TABLE (
    job_title VARCHAR,
    job_location VARCHAR,
    contact_person VARCHAR,
    contact_number VARCHAR,
    functional_skills TEXT,
    technical_skills TEXT,
    job_description TEXT,
    job_type VARCHAR,
    post_date DATE,
    last_date DATE,
    salary MONEY,
    remail varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        jp.job_title, 
        jp.job_location, 
        jp.contact_person, 
        jp.contact_number,
        jp.functional_skills, 
        jp.technical_skills, 
        jp.job_description, 
        jp.job_type,
        jp.post_date, 
        jp.last_date, 
        jp.salary, 
        jp.remail
    FROM JobPostings jp
    WHERE jp.remail = p_remail;
END;
$$;

SELECT * FROM get_job_posting_by_remail('contact@techwizards.com');

---------------------------------------
CREATE OR REPLACE FUNCTION get_job_posting_by_job_title(p_job_title VARCHAR)
RETURNS TABLE (
    job_title VARCHAR,
    company_name VARCHAR,
    job_location VARCHAR,
    job_description TEXT,
	functional_skills TEXT,
    technical_skills TEXT, 
    post_date DATE,
    last_date DATE,
    salary MONEY,
    job_type VARCHAR
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        jp.job_title,
        jp.jp_companyname AS company_name,
        jp.job_location,
        jp.job_description,
		jp.functional_skills,
		jp.technical_skills,
        jp.post_date,
        jp.last_date,
        jp.salary,
        jp.job_type
    FROM
        JobPostings jp
    WHERE
        jp.job_title = p_job_title;
END;
$$ LANGUAGE plpgsql;
 
 
SELECT * FROM get_job_posting_by_job_title('Data Analyst');


-- Create Stored Procedure to Insert Data into JobPostings
CREATE OR REPLACE PROCEDURE insert_job_posting(
    p_job_title VARCHAR(255),
    p_job_location VARCHAR(255),
    p_contact_person VARCHAR(255),
    p_contact_number VARCHAR(15),
    p_functional_skills TEXT,
    p_technical_skills TEXT,
    p_job_description TEXT,
    p_job_type VARCHAR(50),
    p_last_date VARCHAR(50), -- Changed to TEXT to accept date as string
    p_salary int,
    p_remail varchar(50),
	p_jp_companyname varchar(50)
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO JobPostings (
        job_title,
        job_location,
        contact_person,
        contact_number,
        functional_skills,
        technical_skills,
        job_description,
        job_type,
        last_date,
        salary,
        remail,
		jp_companyname
    )
    VALUES (
        p_job_title,
        p_job_location,
        p_contact_person,
        p_contact_number,
        p_functional_skills,
        p_technical_skills,
        p_job_description,
        p_job_type,
        CAST(p_last_date AS DATE), -- Convert string to DATE
        p_salary,
        p_remail,
		p_jp_companyname
    );
END;
$$;

-- Updated Call to Insert Data with Explicit Type Casting
CALL insert_job_posting(
    'Software Engg', 
    'Noida, Uttar Pradesh', 
    'Anita Patel', 
    '9988776655', 
    'Teamwork, Communication', 
    'Java, Python, SQL', 
    'Develop and maintain software applications. Collaborate with other teams to define project requirements.', 
    'Full-time', 
    '2024-10-15', 
    '80000', 
    'hello@alphabytes.com',
	'AlphaBytes'
);


-- Stored Procedure to Update JobPostings with Optional Parameters
CREATE OR REPLACE PROCEDURE update_job_posting(
    p_job_title VARCHAR,
    p_job_location VARCHAR,
    p_contact_person VARCHAR,
    p_contact_number VARCHAR,
    p_functional_skills TEXT,
    p_technical_skills TEXT,
    p_job_description TEXT,
    p_job_type VARCHAR,
    p_last_date VARCHAR(50),
    p_salary int,
	p_remail varchar(50)
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE JobPostings
    SET job_title = p_job_title,
        job_location = p_job_location,
        contact_person = p_contact_person,
		contact_number = p_contact_number,
        functional_skills = p_functional_skills,
		technical_skills = p_technical_skills,
		job_description = p_job_description,
		job_type = p_job_type,
		last_date = CAST(p_last_date AS DATE),
		salary = p_salary
    WHERE remail = p_remail and job_title = p_job_title;
END;
$$;

-- one parameter at a time
CALL update_job_posting(2000, p_job_title := 'New Job Title');

CALL update_job_posting(2000, p_contact_person := 'New Contact Person');

select * from JobPostings;

-- Stored Procedure to Delete Job Postings by Recruiter
CREATE OR REPLACE PROCEDURE delete_job_posting(
    p_job_title varchar,
    p_remail varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM JobPostings
    WHERE job_title = p_job_title AND remail = p_remail;

    IF NOT FOUND THEN
        RAISE NOTICE 'No job posting found with the given job_title and remail.';
    ELSE
        RAISE NOTICE 'Job posting deleted successfully.';
    END IF;
END;
$$;

CALL delete_job_posting('Product Designer', 'info@infosphere.com');


--------------- JobApplications Table ------------------------------
CREATE TABLE JobApplication (
    application_id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY(start with 4000 increment by 1),
    js_email VARCHAR,
    job_id INT,
    application_date DATE NOT NULL DEFAULT CURRENT_DATE,
    status VARCHAR(50) NOT NULL CHECK (status IN ('Pending', 'Accepted', 'Rejected')),
    remail VARCHAR,
    js_resumelink VARCHAR,
    jp_title VARCHAR,
    jp_description VARCHAR,
    r_compname VARCHAR,
    -- Foreign key constraints
    CONSTRAINT fk_js_email FOREIGN KEY (js_email) REFERENCES Jobseeker(email) ON DELETE CASCADE,
    CONSTRAINT fk_job_id FOREIGN KEY (job_id) REFERENCES JobPostings(job_id) ON DELETE CASCADE,
    CONSTRAINT fk_remail FOREIGN KEY (remail) REFERENCES Recruiters(email) ON DELETE CASCADE,
    CONSTRAINT fk_jp_title FOREIGN KEY (jp_title) REFERENCES JobPostings(job_title) ON DELETE CASCADE,
    CONSTRAINT fk_jp_description FOREIGN KEY (jp_description) REFERENCES JobPostings(job_description) ON DELETE CASCADE,
    CONSTRAINT fk_r_compname FOREIGN KEY (r_compname) REFERENCES Recruiters(company_name) ON DELETE CASCADE
);

select * from jobseeker
--- Insert Sample Data into JobApplication Tabl
 
-- Application by Rohit Kumar for Software Developer
INSERT INTO JobApplication (js_email, job_id, application_date, status, remail, js_resumelink, jp_title, jp_description, r_compname)
VALUES ('seema.kumar@example.com', 2001, '2024-09-10', 'Pending', 'contact@techwizards.com', 'http://example.com/resume/rohitkumar', 'Software Developer', 'Develop and maintain software applications. Collaborate with other teams to define project requirements and deliver solutions.', 'TechWizards Pvt Ltd');
 
-- Application by Surya Veer for Data Analyst
INSERT INTO JobApplication (js_email, job_id, application_date, status, remail, js_resumelink, jp_title, jp_description, r_compname)
VALUES ('surya.veer@example.com', 2002, '2024-09-12', 'Rejected', 'support@quantumsoft.com', 'http://example.com/resume/suryaveer', 'Data Analyst', 'Analyze and interpret complex data sets. Create reports and dashboards to support business decision-making.', 'QuantumSoft Technologies');
 
-- Application by Amit Shah for Senior Graphic Designer
INSERT INTO JobApplication (js_email, job_id, application_date, status, remail, js_resumelink, jp_title, jp_description, r_compname)
VALUES ('amit.shah@example.com', 2001, '2024-09-11', 'Accepted', 'info@infosphere.com', 'http://example.com/resume/amitshah', 'Senior Graphic Designer', 'Design marketing materials including brochures, flyers, and digital ads. Collaborate with the marketing team to develop creative concepts.', 'InfoSphere Solutions');
 
-- Application by Ajay Sharma for Project Manager
INSERT INTO JobApplication (js_email, job_id, application_date, status, remail, js_resumelink, jp_title, jp_description, r_compname)
VALUES ('ajay.sharma@example.com', 2000, '2024-09-13', 'Pending', 'contact@techwizards.com', 'http://example.com/resume/ajaysharma', 'Software Developer', 'Develop and maintain software applications. Collaborate with other teams to define project requirements and deliver solutions.', 'TechWizards Pvt Ltd');
 
-- Application by Veer Phariya for Marketing Manager
INSERT INTO JobApplication (js_email, job_id, application_date, status, remail, js_resumelink, jp_title, jp_description, r_compname)
VALUES ('veer.phariya@example.com', 2005, '2024-09-14', 'Pending', 'contact@techwizards.com', 'http://example.com/resume/veerphariya', 'Marketing Manager', 'Plan and execute marketing strategies. Oversee market research and manage the marketing team to drive company growth.', 'TechWizards Pvt Ltd');
 
-- Application by Seema Kumar for HR Manager
INSERT INTO JobApplication (js_email, job_id, application_date, status, remail, js_resumelink, jp_title, jp_description, r_compname)
VALUES ('jane.doess@example.com', 2004, '2024-09-15', 'Pending', 'hello@alphabytes.com', 'http://example.com/resume/seemakumar', 'HR Manager', 'Manage HR functions including recruitment, employee relations, and performance management. Develop and implement HR policies.', 'AlphaBytes');

select * from JobApplication;
select * from jobseeker
select * from recruiters
select * from recruiters
-- Create Function to Retrieve Job Applications by job_id
CREATE OR REPLACE FUNCTION getJobApplicationById(p_application_id INT)
RETURNS TABLE (
    jobseeker_id INT,
    job_id INT,
    application_date DATE,
    status VARCHAR(50),
    recruiter_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT ja.jobseeker_id, ja.job_id, ja.application_date, ja.status, ja.recruiter_id
    FROM JobApplication ja
    WHERE ja.application_id = p_application_id;
END;
$$;
SELECT * FROM getJobApplicationById(4007);

--------------------------------
CREATE OR REPLACE PROCEDURE getJobApplicationsByEmail(seeker_email VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Create a temporary table to hold the results
    CREATE TEMP TABLE temp_job_applications AS
    SELECT 
        a.application_id,
        a.js_email,
        a.job_id,
        a.application_date,
        a.status,
        a.remail,
        a.js_resumelink,
        jp.job_title AS jp_title,
        jp.job_description AS jp_description,
        r.company_name AS r_compname
    FROM 
        JobApplication a
        JOIN JobPostings jp ON a.job_id = jp.job_id
        JOIN Recruiters r ON a.remail = r.email
    WHERE 
        a.js_email = seeker_email;
 
    -- Return the results
    RAISE NOTICE 'Results for job seeker with email %:', seeker_email;
    PERFORM * FROM temp_job_applications;
END;
$$;

call getJobApplicationsByEmail('surya.veer@example.com')


------------
CREATE OR REPLACE PROCEDURE insert_job_application(
     p_job_id INT,
    p_js_email VARCHAR,
    p_status VARCHAR,
    p_js_resumelink VARCHAR,
	p_remail VARCHAR,
    p_jp_title VARCHAR,
    p_jp_description VARCHAR,
    p_r_compname VARCHAR
)

LANGUAGE plpgsql

AS $$

BEGIN

    INSERT INTO JobApplication (

	    job_id,

        js_email,

        status,

        js_resumelink,

		remail,

        jp_title,

        jp_description,

        r_compname

    )

    VALUES (

	    p_job_id,

        p_js_email,           

        p_status,

        p_js_resumelink,

	    p_remail,

        p_jp_title,

        p_jp_description,

        p_r_compname

    );

END;

$$;

CALL insert_job_application(
      '2001',
    'veer.phariya@example.com',   -- p_js_email
    'Rejected',     
	-- p_status
    '
http://example.com/resume.pdf'
, 
	'contact@techwizards.com',-- p_js_resumelink
    'Software Developer',    -- p_jp_title
    'Develop and maintain software applications. Collaborate with other teams to define project requirements and deliver solutions.', -- p_jp_description
    'TechWizards Pvt Ltd'               -- p_r_compname
);


-- Create Stored Procedure to Update Status of Job Application
CREATE OR REPLACE PROCEDURE update_job_application_status(
    p_application_id INT, 
    p_new_status VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Validate the new status value
    IF p_new_status NOT IN ('Pending', 'Accepted', 'Rejected') THEN
        RAISE EXCEPTION 'Invalid status value: %', p_new_status;
    END IF;

    -- Check if the application ID exists
    IF EXISTS (SELECT 1 FROM JobApplication WHERE application_id = p_application_id) THEN
        -- Update the status of the job application
        UPDATE JobApplication
        SET status = p_new_status
        WHERE application_id = p_application_id;

        -- Inform about successful update
        RAISE NOTICE 'Status for application ID % has been updated to %.', p_application_id, p_new_status;
    ELSE
        -- Inform if the application ID does not exist
        RAISE NOTICE 'Application with ID % does not exist.', p_application_id;
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions and provide a message
        RAISE EXCEPTION 'An error occurred while updating status for application ID %: %', p_application_id, SQLERRM;
END;
$$;

CALL update_job_application_status(4000, 'Accepted');

-- Stored Procedure to Insert Data into JobApplication Table
CREATE OR REPLACE PROCEDURE insertApplication(p_jsid INT, p_jid INT, p_appdate DATE,  
                         p_stat VARCHAR, p_recrid INT)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO JobApplication (jobseeker_id, job_id, application_date, status,recruiter_id)  
    VALUES (p_jsid, p_jid, p_appdate, p_stat, p_recrid);
EXCEPTION
    WHEN others THEN
        RAISE NOTICE 'Error inserting job application: %', SQLERRM;
END;
$$;
drop procedure insertApplication

CALL insertApplication(3001, 2001, '2024-09-01', 'Accepted', 1001); 

-----------
CREATE OR REPLACE FUNCTION getJobApplicationsByJobseeker(p_jobseeker_id INT)
RETURNS TABLE (
    application_id INT, jobseeker_id INT, job_id INT, application_date DATE, status VARCHAR, recruiter_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        ja.application_id, ja.jobseeker_id, ja.job_id, ja.application_date, ja.status, ja.recruiter_id
    FROM JobApplication ja
    WHERE ja.jobseeker_id = p_jobseeker_id;
END;
$$;

SELECT * FROM getJobApplicationsByJobseeker(3002);


---------
CREATE OR REPLACE FUNCTION getJobApplicationsByJobId(p_job_id INT)
RETURNS TABLE (
    application_id INT, jobseeker_id INT, job_id INT, application_date DATE, status VARCHAR, recruiter_id INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        ja.application_id, ja.jobseeker_id, ja.job_id, ja.application_date, ja.status, ja.recruiter_id
    FROM JobApplication ja
    WHERE ja.job_id = p_job_id;
END;
$$;

SELECT * FROM getJobApplicationsByJobId(2003);


--------------------------------------------------
CREATE OR REPLACE PROCEDURE Registerrecruiter(p_company_name VARCHAR, p_password VARCHAR, p_email VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Insert new recruiter record
    INSERT INTO Recruiters (company_name, password, email)
    VALUES (p_company_name,p_password, p_email);

    -- Inform about successful insertion
    RAISE NOTICE 'New recruiter % has been successfully inserted.', p_company_name;
EXCEPTION
    WHEN OTHERS THEN
        -- Handle any exceptions and provide a message
        RAISE EXCEPTION 'An error occurred while inserting new recruiter %: %', p_company_name, SQLERRM;
END;
$$;

CALL Registerrecruiter('MS Solutions', 'ms@123' ,'MS@newtech.com');


-----------------------------------------------
CREATE OR REPLACE FUNCTION getallrecruiter()
RETURNS TABLE (
    company_name VARCHAR(255),
    location VARCHAR(255),
    contact_number VARCHAR(15),
    email VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        r.company_name,
        r.location,
        r.contact_number,
        r.email
    FROM
        Recruiters r;
END;
$$;

SELECT * FROM getallrecruiter()

------------- jp --------------------
CREATE OR REPLACE FUNCTION getalljobposting()
RETURNS TABLE (
    job_title VARCHAR,
    job_location VARCHAR,
    contact_person VARCHAR,
    contact_number VARCHAR,
    functional_skills TEXT,
    technical_skills TEXT,
    job_description TEXT,
    job_type VARCHAR,
    post_date DATE,
    last_date DATE,
    salary MONEY,
    remail varchar
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        jp.job_title, 
        jp.job_location, 
        jp.contact_person, 
        jp.contact_number,
        jp.functional_skills, 
        jp.technical_skills, 
        jp.job_description, 
        jp.job_type,
        jp.post_date, 
        jp.last_date, 
        jp.salary, 
        jp.remail
    FROM JobPostings jp;
END;
$$;

SELECT * FROM getalljobposting()


--------- js ---------------------------------------
CREATE OR REPLACE FUNCTION getallJobseekerById()
RETURNS TABLE (name VARCHAR, email VARCHAR, phone_number VARCHAR, address VARCHAR, designation TEXT, education_bg TEXT, work_experience TEXT, skills TEXT, resume_link VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT js.name, js.email, js.phone_number, js.address, js.designation, 
           js.education_bg, js.work_experience, js.skills, js.resume_link
    FROM Jobseeker js;
END;
$$;

-- Example call to get the jobseeker with ID 3000
SELECT * FROM getallJobseekerById()


------------------ ja ---------------------------------------------
CREATE OR REPLACE FUNCTION getJobApplicationByremail(p_remail varchar(255))
RETURNS TABLE (
	js_email VARCHAR(50),
    application_date DATE,
    status VARCHAR(50),
	js_resumelink VARCHAR(255),
	jp_title VARCHAR(50),
	jp_description VARCHAR(255)
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT ja.js_email, ja.application_date, ja.status, ja.js_resumelink, ja.jp_title, ja.jp_description
    FROM JobApplication ja
    WHERE ja.remail = p_remail;
END;
$$;

select * from getJobApplicationByremail('contact@techwizards.com');

------------------------------------------
CREATE OR REPLACE PROCEDURE UpdateJobApplicationStatus(
    IN p_remail VARCHAR,
    IN p_jp_title VARCHAR,
    IN p_status VARCHAR(50)
)
LANGUAGE plpgsql
AS $$
BEGIN
    -- Check if the provided status is valid
    IF p_status NOT IN ('Pending', 'Accepted', 'Rejected') THEN
        RAISE EXCEPTION 'Invalid status value: %', p_status;
    END IF;

    -- Update the status for the given remail and jp_title
    UPDATE JobApplication
    SET status = p_status
    WHERE remail = p_remail
      AND jp_title = p_jp_title;
    
    -- Optional: Check if the update was successful
    IF NOT FOUND THEN
        RAISE NOTICE 'No rows updated for remail: % and jp_title: %', p_remail, p_jp_title;
    END IF;
END;
$$;

CALL UpdateJobApplicationStatus('contact@techwizards.com', 'Software Developer', 'Accepted');


-------------------------------------

CREATE OR REPLACE PROCEDURE updateJobseekerByEmail(p_name VARCHAR, p_email VARCHAR,p_phone VARCHAR,p_addr TEXT,p_desig TEXT,p_edu TEXT,p_work TEXT,p_skills TEXT,p_resume VARCHAR)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE JobApplication SET js_resumelink = p_resume where js_email = p_email;
    UPDATE Jobseeker
    SET 
	    name = p_name,
        phone_number = p_phone,
        address = p_addr,
        designation = p_desig,
        education_bg = p_edu,
        work_experience = p_work,
        skills = p_skills,
        resume_link = p_resume
    WHERE email = p_email; 
END;
$$;
 
 
CALL updateJobseekerByEmail(
    'Rohit kumar',                -- p_name
    'rohit.kumar@example.com',    -- p_email
    '9123456789',                 -- p_phone
    '12, A Block, Sector 15, Noida', -- p_addr
    'Software Engineer',          -- p_desig
    'BTech in Computer Science from IIT Delhi', -- p_edu
    '2 years at TechSolutions Ltd. as a Software Developer', -- p_work
    'Java, Python, SQL',          -- p_skills
    'http://example.com/resume/rohitsheety'
);

---------------------------------------------------------------

CREATE OR REPLACE PROCEDURE deleteJobseekerByEmail(p_email VARCHAR)

LANGUAGE plpgsql

AS $$

BEGIN

    DELETE FROM Jobseeker

    WHERE email = p_email;

END;

$$;
 
 
 
CALL deleteJobseekerByEmail('string');

SELECT * FROM JOBSEEKER
 
CREATE OR REPLACE PROCEDURE getJobApplicationsByEmail(seeker_email VARCHAR)

LANGUAGE plpgsql

AS $$

BEGIN

    -- Create a temporary table to hold the results

    CREATE TEMP TABLE temp_job_applications AS

    SELECT 

        a.application_id,

        a.js_email,

        a.job_id,

        a.application_date,

        a.status,

        a.remail,

        a.js_resumelink,

        jp.job_title AS jp_title,

        jp.job_description AS jp_description,

        r.company_name AS r_compname

    FROM 

        JobApplication a

        JOIN JobPostings jp ON a.job_id = jp.job_id

        JOIN Recruiters r ON a.remail = r.email

    WHERE 

        a.js_email = seeker_email;
 
    -- Return the results

    RAISE NOTICE 'Results for job seeker with email %:', seeker_email;

    PERFORM * FROM temp_job_applications;

END;

$$;
 
CALL getJobApplicationsByEmail('rohit.kumar@example.com');


